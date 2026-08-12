using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SBR.Forms;

public partial class UcAbout : UserControl
{

    List<MyApps> myApps = new List<MyApps>(); // here data from the MyApps.xml file will be stored
    string html_text;

    public UcAbout()
    {
        InitializeComponent();
        richTextBoxAbout.MouseClick += RichTextBoxAbout_MouseClick;
        ReadXmlMyApps();
        Debug.Print("XML read");
    }


 
    /// <summary>
    /// Method will read MyApps.xml file and store data in myApps list.
    /// </summary>
    private void ReadXmlMyApps()
    {
        // Only load if list is empty
        if (myApps.Count > 0)
            return;

        try
        {
            // Path to MyApps.xml - in the root of the project (copied to bin folder at build)
            string xmlPath = Path.Combine(Application.StartupPath, "MyApps.xml");

            if (!File.Exists(xmlPath))
            {
                return;
            }

            XDocument doc = XDocument.Load(xmlPath);
            var apps = doc.Root?.Elements("app");

            if (apps == null)
            {
                return;
            }

            foreach (var app in apps)
            {
                var title = app.Element("title")?.Value ?? string.Empty;
                var url = app.Element("url")?.Value ?? string.Empty;
                var description = app.Element("description")?.Value ?? string.Empty;
                var screenshotName = app.Element("screenshotName")?.Value ?? string.Empty;

                if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(url))
                {
                    myApps.Add(new MyApps
                    {
                        Title = title,
                        Url = url,
                        Description = description,
                        ScreenshotName = screenshotName
                    });
                }
            }

            DisplayMyApps();    // Display the apps on the form
        }
        catch (Exception ex)
        {

            MessageBox.Show($"Error loading MyApps.xml: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Method will display the apps in the myApps list on the form.
    /// </summary>
    private void DisplayMyApps()
    {

        if (myApps.Count == 0)
            return;

        FlowLayoutPanel? container = pnlAppsContainer; // name of the FlowLayoutPanel in your UserControl

        if (container == null)
        {
            return;
        }

        container.Controls.Clear();

        foreach (var app in myApps)
        {
            Panel appPanel = new Panel
            {
                Size = new Size(280, 400), // 250
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 0, 10),
                BackColor = Color.White
            };

            int yPos = 10;

            // Add title as clickable LinkLabel
            LinkLabel lblTitle = new LinkLabel
            {
                Text = app.Title,
                AutoSize = true,
                Location = new Point(10, yPos),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold | FontStyle.Underline),
                LinkColor = Color.Blue,
                ActiveLinkColor = Color.Red,
                VisitedLinkColor = Color.Purple,
                Tag = app.Url,
                MaximumSize = new Size(280, 0) // 230
            };

            lblTitle.LinkClicked += (s, e) =>
            {
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = app.Url,
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                }
                catch { }
            };

            appPanel.Controls.Add(lblTitle);
            yPos += lblTitle.Height + 10;

            // Add screenshot from resources
            if (!string.IsNullOrEmpty(app.ScreenshotName))
            {
                try
                {
                    // Get image from Resources (Properties.Resources)
                    Image? screenshot = (Image?)SBR.ResourcesMyApps.ResourcesMyApps.ResourceManager.GetObject(app.ScreenshotName);

                    if (screenshot != null)
                    {
                        int newWidth = 258;
                        int newHeight = (int)(screenshot.Height * (newWidth / (double)screenshot.Width));
                        Image resizedImage = new Bitmap(screenshot, new Size(newWidth, newHeight));

                        PictureBox pbScreenshot = new PictureBox
                        {
                            Image = resizedImage,
                            SizeMode = PictureBoxSizeMode.AutoSize,
                            Location = new Point(10, yPos)
                        };

                        appPanel.Controls.Add(pbScreenshot);
                        yPos += pbScreenshot.Height + 10;
                    }
                }
                catch (Exception ex)
                { 
                    MessageBox.Show($"Error loading screenshot for {app.Title}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // Add description
            if (!string.IsNullOrEmpty(app.Description))
            {
                Label lblDescription = new Label
                {
                    Text = app.Description,
                    Location = new Point(10, yPos),
                    Size = new Size(280, 0),
                    AutoSize = true,
                    MaximumSize = new Size(280, 0),
                    Font = new Font("Segoe UI", 9F),
                    BackColor = Color.White
                };

                appPanel.Controls.Add(lblDescription);
                yPos += lblDescription.Height + 10;
            }

            appPanel.Height = yPos;
            container.Controls.Add(appPanel);
        }

        lblAbout6.Visible = true;
    }




    // ********************************************************************************************************************
    // ** Public Methods:
    // ********************************************************************************************************************
    public void ChangeLanguage()
    {
        // List of strings which are in current User Control (Windows Form) and which we want to change to different language.
        //// There is such method in each User Control (Windows Form) which is called from LangChanger static class.

        lblAbout6.Text = LangManager.GetString("free_apps");
        html_text = LangManager.GetString("about_html");
        ParseHtml(html_text);
    }


    // ********************************************************************************************************************
    // ** Private Methods:
    // ********************************************************************************************************************


    private Dictionary<(int start, int end), string> linkMap = new Dictionary<(int, int), string>();

    private void ParseHtml(string? htmlText)
    {

        if (string.IsNullOrEmpty(htmlText))
            return;

        richTextBoxAbout.Clear();
        linkMap.Clear();
        richTextBoxAbout.SelectionIndent = 0;


        string workingText = htmlText;
        int position = 0;

        // Main loop to parse the HTML string and apply formatting to the RichTextBox
        while (position < workingText.Length)
        {
            int tagStart = workingText.IndexOf('<', position);

            // No tag found, append the rest of the text and break the loop
            if (tagStart == -1)
            {
                if (position < workingText.Length)
                {
                    richTextBoxAbout.AppendText(workingText.Substring(position));
                }
                break;
            }

            // Tag found, append text (to richBox) before the tag
            if (tagStart > position)
            {
                richTextBoxAbout.AppendText(workingText.Substring(position, tagStart - position));
            }

            int tagEnd = workingText.IndexOf('>', tagStart);
            if (tagEnd == -1) break;

            // Extract the tag and its content
            string tag = workingText.Substring(tagStart, tagEnd - tagStart + 1);

            if (tag.Equals("<br>", StringComparison.OrdinalIgnoreCase))
            {
                richTextBoxAbout.AppendText("\n");
            }
            else if (tag.Equals("<hr>", StringComparison.OrdinalIgnoreCase))
            {
                richTextBoxAbout.AppendText("\n" + new string('.', 145) + "\n");
            }
            else if (tag.StartsWith("<strong>", StringComparison.OrdinalIgnoreCase))
            {
                int closeTagStart = workingText.IndexOf("</strong>", tagEnd, StringComparison.OrdinalIgnoreCase);
                if (closeTagStart != -1)
                {
                    string boldText = workingText.Substring(tagEnd + 1, closeTagStart - tagEnd - 1);
                    richTextBoxAbout.SelectionFont = new Font(richTextBoxAbout.Font, FontStyle.Bold);
                    richTextBoxAbout.AppendText(boldText);
                    richTextBoxAbout.SelectionFont = new Font(richTextBoxAbout.Font, FontStyle.Regular);
                    position = closeTagStart + "</strong>".Length;
                    continue;
                }
            }
            else if (tag.StartsWith("<a href='", StringComparison.OrdinalIgnoreCase))
            {
                int urlStart = tag.IndexOf("'") + 1;
                int urlEnd = tag.IndexOf("'", urlStart);
                string url = tag.Substring(urlStart, urlEnd - urlStart);

                int closeTagStart = workingText.IndexOf("</a>", tagEnd, StringComparison.OrdinalIgnoreCase);
                if (closeTagStart != -1)
                {
                    string linkText = workingText.Substring(tagEnd + 1, closeTagStart - tagEnd - 1);

                    // Store the link position before appending
                    int linkStartPos = richTextBoxAbout.TextLength;
                    //richTextBoxAbout.SelectionColor = Color.Black;
                    richTextBoxAbout.SelectionFont = new Font(richTextBoxAbout.Font, FontStyle.Underline);
                    richTextBoxAbout.AppendText(linkText);
                    int linkEndPos = richTextBoxAbout.TextLength;

                    // Track this link
                    linkMap[(linkStartPos, linkEndPos)] = url;

                    //richTextBoxAbout.SelectionColor = Color.Black;
                    richTextBoxAbout.SelectionFont = new Font(richTextBoxAbout.Font, FontStyle.Regular);
                    position = closeTagStart + "</a>".Length;
                    continue;
                }
            }

            position = tagEnd + 1;
        }

        richTextBoxAbout.SelectionStart = 0;
    }

    private void RichTextBoxAbout_MouseClick(object sender, MouseEventArgs e)
    {
        int charIndex = richTextBoxAbout.GetCharIndexFromPosition(e.Location);

        foreach (var link in linkMap)
        {
            if (charIndex >= link.Key.start && charIndex < link.Key.end)
            {
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = link.Value,
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening link: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }
        }
    }







}
