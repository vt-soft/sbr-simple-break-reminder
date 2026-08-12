using Microsoft.Win32.SafeHandles;
using SBR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Resources;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace SBR.Forms
{
    public partial class UcLanguage : UserControl
    {
        // Here data from _credits_info.json will be stored.
        private List<CreditsInfo> creditList = new(); 

        private FrmTranslators myForm;
        public UcLanguage()
        {
            InitializeComponent();
           
            LoadJsonCreditsInfoFile();
            UpdateComboBoxWithLanguages();
            PopulatePanelWithCredits();

            EventHandlersInit();
        }

        // ********************************************************************************************************************
        // ** Public Methods:
        // ********************************************************************************************************************

        /// <summary>
        /// Method will change strings in current User Control (Windows Form) to proper language.
        /// </summary>
        public void ChangeLanguage()
        {
            // List of strings which are in current User Control (Windows Form) and which we want to change to different language.
            // There is such method in each User Control (Windows Form) which is called from LangChanger static class.
            lblSelectLanguage.Text = LangManager.GetString("select_lang");
            lblCredits.Text = LangManager.GetString("credits");
        }


        // ********************************************************************************************************************
        // ** Private Methods:
        // ********************************************************************************************************************


        private void EventHandlersInit()
        {
            cboLanguages.SelectedIndexChanged += cboLanguages_SelectedIndexChanged;
            btnMissingLanguage.Click += btnMissingLanguage_Click;
        }


        /// <summary>
        /// Load _sbr_credits_info.json file and populate the creditList
        /// </summary>
        private void LoadJsonCreditsInfoFile()
        {
            try
            {
                // Construct the path to the file _sbr_credits_info.json
                string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string jsonFilePath = Path.Combine(appDirectory, "Languages", "_sbr_credits_info.json");

                // Check if file exists
                if (!File.Exists(jsonFilePath))
                {
                    MessageBox.Show($"File not found: {jsonFilePath}");
                    return;
                }

                // Read the JSON file
                string jsonContent = File.ReadAllText(jsonFilePath);

                // Parse JSON and extract id and translated_string
                using (JsonDocument doc = JsonDocument.Parse(jsonContent))
                {
                    JsonElement root = doc.RootElement;

                    // Handle if root is an array
                    if (root.ValueKind == JsonValueKind.Array)
                    {
                        foreach (JsonElement item in root.EnumerateArray())
                        {
                            if (item.TryGetProperty("language_code", out JsonElement lcElement) &&
                                item.TryGetProperty("country_language", out JsonElement clElement) &&
                                item.TryGetProperty("name", out JsonElement nameElement) &&
                                item.TryGetProperty("hyper_link_url", out JsonElement hlElementUrl) &&
                                item.TryGetProperty("hyper_link_text", out JsonElement hlElementText))
                            {
                                string languageCode = lcElement.GetString();
                                string countryLanguage = clElement.GetString();
                                string name = nameElement.GetString();
                                string hyperLinkUrl = hlElementUrl.GetString();
                                string hyperLinkText = hlElementText.GetString();

                                creditList.Add(new CreditsInfo
                                {
                                    LanguageCode = languageCode,
                                    CountryLanguage = countryLanguage,
                                    Name = name,
                                    HyperLinkUrl = hyperLinkUrl,
                                    HyperLinkText = hyperLinkText
                                });

                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading JSON file: {ex.Message}");
            }
        }

        /// <summary>
        /// Method will populate pnlTableLang with flags, credits and hyperlinks.
        /// </summary>
        private void PopulatePanelWithCredits()
        {
            pnlTableLang.Controls.Clear();
            pnlTableLang.RowCount = 0;

            Image flag = null;

            for (int i = 0; i < creditList.Count; i++)
            {
                flag = FindFlag(creditList[i].LanguageCode);
                int flagWidth = flag != null ? flag.Width : 0;
                int flagHeight = flag != null ? flag.Height : 0;

                // Create PictureBox for flag (left-aligned)
                PictureBox pictureBox = new PictureBox
                {
                    Image = flag,
                    Height = (flagHeight - 2) * 2 / 3, // flags are too big, so we scale them down to 2/3 of their original height
                    Width = (flagWidth - 2) * 2 / 3,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Anchor = AnchorStyles.Left | AnchorStyles.Top,
                };

                // Create RichTextBox
                RichTextBox richTextBox = new RichTextBox
                {
                    Width = 650,
                    Height = 50,
                    ReadOnly = true,
                    BorderStyle = BorderStyle.None,
                    BackColor = SystemColors.Control,
                    Font = new Font("Segoe UI", 9.75f),
                    Cursor = Cursors.Default,
                    DetectUrls = true
                };

                // Build the text with country language, name, and link
                richTextBox.Text = $"{creditList[i].CountryLanguage}\n{creditList[i].Name} | ";

                int linkStart = richTextBox.Text.Length;
                string linkUrl = creditList[i].HyperLinkUrl;
                string linkDisplayText = creditList[i].HyperLinkText;

                // Append the display text
                richTextBox.AppendText(linkDisplayText);
                richTextBox.Select(linkStart, linkDisplayText.Length);
                richTextBox.SelectionFont = new Font("Segoe UI", 9.75f, FontStyle.Underline);

                Color linkColor = ControlPaint.Light(SystemColors.ControlText, 0.7f);
                richTextBox.SelectionColor = linkColor;
                richTextBox.DeselectAll();

                // Handle link click
                richTextBox.MouseClick += (sender, e) =>
                {
                    // Check if click is within the link text
                    int clickPosition = richTextBox.GetCharIndexFromPosition(e.Location);
                    if (clickPosition >= linkStart && clickPosition < linkStart + linkDisplayText.Length)
                    {
                        if (!string.IsNullOrEmpty(linkUrl))
                        {
                            try
                            {
                                var psi = new ProcessStartInfo
                                {
                                    FileName = linkUrl,
                                    UseShellExecute = true
                                };
                                Process.Start(psi);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Could not open link: {ex.Message}");
                            }
                        }
                    }
                };

                // Add controls to the TableLayoutPanel
                pnlTableLang.Controls.Add(pictureBox, 0, pnlTableLang.RowCount);
                pnlTableLang.Controls.Add(richTextBox, 1, pnlTableLang.RowCount);

                // No need to run rest of this code for last item, otherwise it will create extra blank table.
                if (i == (creditList.Count - 1)) break;

                pnlTableLang.RowCount++;  //increase panel rows count by one

                //Add a new RowStyle as a copy of the previous one.
                RowStyle temp = pnlTableLang.RowStyles[0];
                pnlTableLang.RowStyles.Add(new RowStyle(temp.SizeType, temp.Height));

                // Increase height of the panel.     
                if (pnlTableLang.Height <= 280)
                {
                    pnlTableLang.Height += 70;  // Increased from 55 to 70
                }
            }

            // Trick to disable horizontal scrollbar.
            int vertScrollWidth = SystemInformation.VerticalScrollBarWidth;
            pnlTableLang.Padding = new Padding(0, 0, vertScrollWidth, 0);
        }

        
        /// <summary>
        /// Method will find flag (from resources) by language code.
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        private Image FindFlag(string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode))
                return ResourcesFlagsDir.ResourcesFlags.empty_flag;

            // Convert language code format from "xx-xx" to "xx_xx"
            string flagName = languageCode.Replace("-", "_").ToLower();

            // Use reflection to get the property dynamically from ResourcesFlagsDir.ResourcesFlags
            var flagsType = typeof(ResourcesFlagsDir.ResourcesFlags);
            var property = flagsType.GetProperty(flagName,
                System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            if (property != null && property.CanRead)
            {
                var flagImage = property.GetValue(null) as Image;
                return flagImage ?? ResourcesFlagsDir.ResourcesFlags.empty_flag;
            }

            return ResourcesFlagsDir.ResourcesFlags.empty_flag;
        }


        /// <summary>
        /// Method will update combobox with languages (from creditList).
        /// </summary>
        private void UpdateComboBoxWithLanguages()
        {
     
            cboLanguages.Items.Clear();
            foreach (var credit in creditList)
            {
                cboLanguages.Items.Add(credit.CountryLanguage);
            }

            //Sort the list of languages in the combobox alphabetically.
            cboLanguages.Sorted = true;

            // Change the selected language in the combobox to the current language of the application.
            // This part is here because after we load setting file we want to have the correct language selected in the combobox.
            string langCode = MainForm.MainFormInstance?.cData?.LanguageCode;

            if (!string.IsNullOrEmpty(langCode))
            {
                cboLanguages.SelectedItem = creditList.FirstOrDefault(c => c.LanguageCode == langCode)?.CountryLanguage;
            }

            // Set picture box to the flag of the selected language in the combobox.
            picFlag.Image = FindFlag(creditList.FirstOrDefault(c => c.CountryLanguage == cboLanguages.SelectedItem?.ToString())?.LanguageCode);

        }

        /// <summary>
        /// Method will change language and will update flag image when different language is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Ensure SelectedItem is not null before using it
            if (cboLanguages.SelectedItem != null)
            {
                string selectedLanguage = cboLanguages.SelectedItem.ToString();
                foreach (var credit in creditList)
                {
                    if (credit.CountryLanguage == selectedLanguage)
                    {
                        // Change the flag image.
                        picFlag.Image = FindFlag(credit.LanguageCode);

                        // Change the language in the application.
                        LangManager.SetLanguage(credit.LanguageCode);

                        // Change the languageCode in the settings file.
                        MainForm.MainFormInstance.cData.LanguageCode = credit.LanguageCode;
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// Method will open FrmTranslators form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMissingLanguage_Click(object sender, EventArgs e)
        {
            if (myForm == null || myForm.IsDisposed)
            {
                myForm = new FrmTranslators();
                myForm.StartPosition = FormStartPosition.CenterParent;
                myForm.FormClosed += (s, args) => myForm = null; // reset myForm when the form is closed
                myForm.ShowDialog(this); // use ShowDialog instead of Show to ensure it is centered relative to the parent
            }

        }




    }
}
