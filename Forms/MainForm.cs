using SBR;
using SBR.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
// using System.Reflection.Emit;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.LinkLabel;
using Microsoft.Win32;
using Windows.ApplicationModel.VoiceCommands;


// **************************************************************************************************************************
// *  Project information:                                                                                                  *
// *                                                                                                                        *
// *  Name        :  SBR - Simple Break Reminder                                                                            *
// *  Description :  Application reminding you to take a break when working with computer                                   *
// *  Language    :  C# 14                                                                                                  *
// *  Framework   :  .NET 10.0                                                                                              *
// *  UI          :  WinForms                                                                                               *
// *  NuGet       :  https://www.nuget.org/packages/WinForms.DataVisualization                                              *
// *  Icons       :  By Icons8 (https://icons8.com/)                                                                        *
// *  Web         :  https://www.vt-soft.com/sbr-simple-break-reminder                                                      *
// *                                                                                                                        *
// *  Please be aware that I am not professional developer so this code is not perfect                                      *
// *  and it is probably not following all best programming practices.                                                      *
// *  However I still hope that you will find it useful and that it will help you to create your own application.           *
// *  For more projects please check https://www.vt-soft.com/                                                               *
// *  Any link to this site is highly appreciated. Enjoy the code! :)                                                       *
// *                                                                                                                        *
// *  Copyright(c) 2025-2026, vt-soft                                                                                       *
// *  All rights reserved.                                                                                                  *
// *                                                                                                                        *
// *  This source code is licensed under the MIT-style license.                                                             *
// *  More info in the license.txt file in the root directory of this source tree.                                          *
// **************************************************************************************************************************




namespace SBR;

public partial class MainForm : Form
{

    public static MainForm MainFormInstance;

    public static bool TimeIsUp = false;    // Just for color purposes.
    public static int PomoCounter = 0;

    public bool DarkMode { get; private set; } = false;    // Day/dark mode

    private Color navBarBacgroundColor1 = Color.FromArgb(41, 39, 40);   // Color for selected left NavBar button.
    private Color navBarBacgroundColor2 = Color.Gray;                   // Color for non-selected NavBar button.

    // We store all UserControl (Forms) in this dictionary.
    private Dictionary<string, UserControl> screens = new Dictionary<string, UserControl>();

    // This is the current User Control (Windows Form) which is loaded into the pnlFormLoader panel.
    private UserControl currentControl;

    // Here we store all necessary data. These objects are also then saved to json files.
    public DataConfig? cData;

    public MainForm()
    {
        InitializeComponent();
        MainFormInstance = this;
    }

    // ********************************************************************************************************************
    // ** Public Methods:
    // ********************************************************************************************************************




    /// <summary>
    /// Method for adjusting the color of the horizontal and vertical strip.
    /// </summary>
    public void SetStripsColor()
    {
        pnlVerticalStrip.BackColor = ColorTranslator.FromHtml(cData.SelectedColor);
        pnlHorizontalStrip.BackColor = ColorTranslator.FromHtml(cData.SelectedColor);
        btnChangeDayMode.ForeColor = ColorTranslator.FromHtml(cData.SelectedColor);
    }


    /// <summary>
    /// Method for changing strings in current User Control (Windows Form) to proper (selected) language.
    /// </summary>
    public void ChangeLanguage()
    {
        // List of strings which are in current User Control (Windows Form) and which we want to change to different language.
        // This method ChangeLanguage() in each User Control (Windows Form) and is called from LangChanger static class.
        btnAlarm.Text = "  " + LangManager.GetString("alarm");
        btnSettings.Text = "  " + LangManager.GetString("settings");
        btnStatistics.Text = "  " + LangManager.GetString("statistics");
        btnLanguage.Text = "  " + LangManager.GetString("language");
        btnAbout.Text = "  " + LangManager.GetString("about");
    }

    /// <summary>
    /// Method for loading the correct Form (User Control) into the pnlFormLoader panel.
    /// </summary>
    /// <param name="buttonName"></param>
    public void LoadUCForm(string buttonName)
    {
        this.pnlFormLoader.SuspendLayout(); // Suspend layout to prevent redraw. Probably not necessary here :)

        //  Without this part of code there were some white flashes when switching between Forms (User Controls).
        if (DarkMode)
        {
            this.BackColor = Color.FromArgb(80, 80, 80); // dark mode
        }
        else
        {
            this.BackColor = SystemColors.Control; // day mode
        }

        this.pnlFormLoader.Controls.Clear();
        currentControl = screens[buttonName];     // Searching for the proper Form (User Control) in the Dictionary.
        currentControl.Dock = DockStyle.Fill;     // Ensure the UserControl is docked correctly.
        this.pnlFormLoader.Controls.Add(currentControl);
        this.pnlFormLoader.ResumeLayout();        // Resume layout after adding the control. Probably not necessary here :)
        currentControl.Show();

    }


    /// <summary>
    /// Method for changing the color of the button when it is enabled or disabled.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void ButtonEnabledChanged(object sender, EventArgs e)
    {
        // There is little chaos in Dark/Day mode for buttons. But I am rather not touching it anymore :)

        Button b = (Button)sender;

        if (DarkMode)
        {
            b.BackColor = Color.Gray;
            b.ForeColor = Color.WhiteSmoke;
        }
        else
        {
            b.BackColor = SystemColors.ButtonHighlight;
            b.ForeColor = SystemColors.ControlText;

            if (b.Enabled == false)
            {
                b.BackColor = SystemColors.ButtonFace;
            }
        }

    }

    /// <summary>
    /// Method for switching the application to the system tray and back based on the config data.
    /// </summary>
    public void SwitchToTrayAndBack()
    {
        if (cData?.SystemTray == true)
        {
            this.ShowInTaskbar = false;
            notifyIconTray.Visible = true;
        }
        else
        {
            this.ShowInTaskbar = true;
            notifyIconTray.Visible = false;
        }
    }


    // ********************************************************************************************************************
    // ** Private Methods:
    // ********************************************************************************************************************


    /// <summary>
    /// Entry point of the application. This method is called when the MainForm is loaded.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MainForm_Load(object sender, EventArgs e)
    {
        this.WindowState = FormWindowState.Minimized;
        MainFormInit();
    }

    private void notifyIconTray_MouseClick(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            if (this.Visible && this.WindowState != FormWindowState.Minimized)
            {
                this.Hide();
            }
            else
            {

                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.Activate();
            }
        }
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);

        // If system-tray mode is enabled, hide the form when minimized so it completely
        // disappears into the tray (no small/minimized window left on desktop).
        if (cData?.SystemTray == true)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.ShowInTaskbar = false;
                notifyIconTray.Visible = true;
            }
            else if (this.WindowState == FormWindowState.Normal)
            {
                // When restored, ensure the form and taskbar icon are shown.
                this.Show();
                this.ShowInTaskbar = true;
                notifyIconTray.Visible = true;
                this.Activate();
            }
        }
    }


    private void MainFormInit()
    {
        // Load config data from json file
        cData = ConfigManager.LoadJsonConfigFile();

        SwitchToTrayAndBack(); // Show or hide the application in the system tray based on the config data.

        //Method which will populate the "screens" dictionary with all User Control(WinForms).
        FormsAndButtonsInit();

        // Pass on references to the MainForm and other Forms to LangChanger static class
        LangManager.Init(this, screens);

        // Adjust the language according to the value in the json file.
        LangManager.SetLanguage(cData.LanguageCode);

        // Load correct Form (btnAlarm in this case) into the pnlFormLoader panel.
        LoadUCForm("btnAlarm");


        // Adjust the dark mode according to the value in the json file.
        if (cData.DarkMode)
        {
            btnChangeDayMode.Image = ResourcesIconsDir.ResourcesIcons.sun_icon;
            DarkModeOn();
        }
        else
        {
            btnChangeDayMode.Image = ResourcesIconsDir.ResourcesIcons.moon_icon;
            DarkModeOff();
        }

        // Adjust color of the horizontal and vertical strips.
        SetStripsColor();
    }

    /// <summary>
    /// Method for initializing the NavBar buttons and Forms (User Controls).    
    /// </summary>       
    private void FormsAndButtonsInit()
    {
        NavBarButton_Leave();

        // Initialize the vertical strip to first button (btnAlarm).
        pnlVerticalStrip.Height = btnAlarm.Height;
        pnlVerticalStrip.Top = btnAlarm.Top;
        pnlVerticalStrip.Left = 0;
        btnAlarm.BackColor = navBarBacgroundColor2;


        // For Event purposes  we are creating ucAlarm and ucSettings here in a bit different way.
        UcAlarm ucAlarm = new UcAlarm() { Dock = DockStyle.Fill };
        UcSettings ucSettings = new UcSettings() { Dock = DockStyle.Fill };

        // Subscribe to the events
        ucAlarm.SubscribeToEvents(ucSettings);

        // Populate the dictionary list (Key-Value) with all Forms (User Control).
        // Key (like btnAlarm) must exactly match button name.
        screens.Add("btnAlarm", ucAlarm);
        screens.Add("btnSettings", ucSettings);
        screens.Add("btnStatistics", new UcStatistics() { Dock = DockStyle.Fill });
        screens.Add("btnLanguage", new UcLanguage() { Dock = DockStyle.Fill });
        screens.Add("btnAbout", new UcAbout() { Dock = DockStyle.Fill });
    }

    /// <summary>
    /// Method for handling the NavBar button click event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NavBarButton_Click(Object sender, EventArgs e)
    {
        // Instead of having a separate event handler for each button we have one general event for all NavBar buttons.
        // This event is triggered when any NavBar button is clicked.

        Button button = (Button)sender;

        // Check if the form associated with the clicked button is already loaded
        if (currentControl == screens[button.Name])
        {
            return; // Form is already loaded, no need to reload
        }

        NavBarButton_Leave();

        // Adjust the NavBarPanel to the clicked button.
        pnlVerticalStrip.Height = button.Height;
        pnlVerticalStrip.Top = button.Top;
        pnlVerticalStrip.Left = 0;
        button.BackColor = navBarBacgroundColor2;

        // Load correct Form into the pnlFormLoader panel.
        LoadUCForm(button.Name);
    }

    /// <summary>
    /// Method for handling the NavBar button mouse enter event. 
    /// When mouse leaves the NavBar button, hide the gray vertical strip.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NavBarButton_MouseLeave(object sender, EventArgs e)
    {
        pnlVerticalStrip2.Visible = false;
    }

    /// <summary>
    /// Method for handling the NavBar button mouse enter event.
    /// When mouse enters the NavBar button, show the gray vertical strip.   
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NavBarButton_MouseEnter(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        pnlVerticalStrip2.Visible = true;
        pnlVerticalStrip2.Top = button.Top;
    }

    // <summary>
    // Method for resetting the colors of all NavBar buttons.
    // </summary>
    private void NavBarButton_Leave()
    {
        btnAlarm.BackColor = navBarBacgroundColor1;

        btnSettings.BackColor = navBarBacgroundColor1;
        btnStatistics.BackColor = navBarBacgroundColor1;
        btnLanguage.BackColor = navBarBacgroundColor1;
        btnAbout.BackColor = navBarBacgroundColor1;
    }




    // ************************************************************************************************
    // *  Methods for dark / day mode.
    // *

    /// <summary>
    /// Method for changing the application to dark mode and back to day mode.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnChangeDayMode_Click(object sender, EventArgs e)
    {
        DarkMode = !DarkMode;

        if (DarkMode)
        {
            btnChangeDayMode.Image = ResourcesIconsDir.ResourcesIcons.sun_icon;
            DarkModeOn();
        }
        else
        {
            btnChangeDayMode.Image = ResourcesIconsDir.ResourcesIcons.moon_icon;
            DarkModeOff();
        }
    }

    /// <summary>
    /// Method for changing the application to dark mode.
    /// </summary>
    private void DarkModeOn()
    {
        cData.DarkMode = DarkMode = true;
        Color myBackgroundColor = Color.FromArgb(80, 80, 80);
        Color myForeColor = Color.WhiteSmoke;

        foreach (var item in screens.Values)      // going through all Forms (User Controls)
        {
            item.BackColor = myBackgroundColor;   // color of the form (User Control) background

            foreach (Control c in item.Controls)  // going through all Controls in particular Form (User Control)
            {
                if (c is Label)
                {
                    Label l = (Label)c;
                    l.ForeColor = myForeColor;
                    l.BackColor = myBackgroundColor;

                    if (TimeIsUp && l.Name == "lblAlarmTime")
                    {
                        l.ForeColor = Color.FromArgb(238, 70, 90);
                    }
                }
                else if (c is Button)
                {
                    Button b = (Button)c;
                    b.ForeColor = myForeColor;
                    b.BackColor = Color.Gray;

                    if (b.Enabled == false)
                    {
                        // b.BackColor = SystemColors.ControlDark;
                        b.ForeColor = Color.WhiteSmoke;
                    }

                    if (TimeIsUp && (b.Name == "btnBreakYes" || b.Name == "btnBreakNo"))
                    {
                        b.ForeColor = SystemColors.ControlText;

                        if (b.Name == "btnBreakYes")
                            b.BackColor = Color.LightGreen;
                        if (b.Name == "btnBreakNo")
                            b.BackColor = Color.FromArgb(255, 153, 163); // Light pink  
                    }
                    if (TimeIsUp && PomoCounter == 4 && b.Name == "btnPomodoro")
                    {
                        b.BackColor = Color.FromArgb(238, 70, 90);
                    }

                }
                else if (c is ComboBox)
                {
                    ComboBox cb = (ComboBox)c;
                    cb.ForeColor = myForeColor;
                    cb.BackColor = myBackgroundColor;
                }
                else if (c is RichTextBox)
                {
                    RichTextBox r = (RichTextBox)c;
                    r.ForeColor = myForeColor;
                    r.BackColor = myBackgroundColor;
                    r.SelectionColor = myForeColor;
                }

                else if (c is Panel) // will work both for Panel and TableLayoutPanel
                {
                    DarkModeOnTLP((Panel)c, myForeColor, myBackgroundColor);
                }
                else if (c is Chart)
                {
                    DarkModeOnCharts((Chart)c, myForeColor, myBackgroundColor);
                }

            }
        }
    }



    /// <summary>
    /// Method for changing the application to dark mode in TableLayoutPanel and Panel.
    /// </summary>
    /// <param name="tlp"></param>
    /// <param name="myForeColor2"></param>
    /// <param name="myBackgroundColor2"></param>
    private void DarkModeOnTLP(Panel tlp, Color myForeColor, Color myBackgroundColor)
    {
        Color myLinkForeColor = Color.Silver;

        foreach (Control c2 in tlp.Controls)
        {
            // Warning: as LinkLabel is derived from Label, it must be checked first!
            if (c2 is LinkLabel)
            {
                LinkLabel ll = (LinkLabel)c2;
                ll.LinkColor = myLinkForeColor;
                ll.ActiveLinkColor = myLinkForeColor;
                ll.VisitedLinkColor = myLinkForeColor;
                ll.ForeColor = myLinkForeColor;
                ll.BackColor = myBackgroundColor;
            }
            else if (c2 is Label)
            {
                Label l = (Label)c2;
                l.ForeColor = myForeColor;
                l.BackColor = myBackgroundColor;
            }
            else if (c2 is RadioButton)
            {
                RadioButton r = (RadioButton)c2;
                r.ForeColor = myForeColor;
                r.BackColor = myBackgroundColor;
            }
            else if (c2 is CheckBox)
            {
                CheckBox ch = (CheckBox)c2;
                ch.ForeColor = myForeColor;
                ch.BackColor = myBackgroundColor;
            }
            else if (c2 is NumericUpDown)
            {
                NumericUpDown n = (NumericUpDown)c2;
                n.ForeColor = myForeColor;
                n.BackColor = myBackgroundColor;
            }

            else if (c2 is RichTextBox)
            {
                RichTextBox r = (RichTextBox)c2;
                r.ForeColor = myForeColor;
                r.BackColor = myBackgroundColor;
                r.SelectionColor = myForeColor;
            }

        }
    }

    private void DarkModeOnCharts(Chart chart, Color myForeColor, Color myBackgroundColor)
    {

        Color lighterBColor = Color.FromArgb(
            Math.Min(myBackgroundColor.R + 20, 255), // Ensure the value doesn't exceed 255
            Math.Min(myBackgroundColor.G + 20, 255),
            Math.Min(myBackgroundColor.B + 20, 255)
        );

        // Set color of the chart background
        chart.BackColor = myBackgroundColor;
        chart.ChartAreas[0].BackColor = lighterBColor;

        // Set color of the axes 
        chart.ChartAreas[0].AxisX.LineColor = myForeColor;
        chart.ChartAreas[0].AxisY.LineColor = myForeColor;
        chart.ChartAreas[0].AxisY2.LineColor = myForeColor;

        // Set color of the axes labels
        chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = myForeColor;
        chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = myForeColor;
        chart.ChartAreas[0].AxisY2.LabelStyle.ForeColor = myForeColor;

        // Set color of the tick marks (next to the Y-axis labels)
        chart.ChartAreas[0].AxisY.MajorTickMark.LineColor = myForeColor;
        chart.ChartAreas[0].AxisY.MinorTickMark.LineColor = myForeColor;
        chart.ChartAreas[0].AxisY2.MajorTickMark.LineColor = myForeColor;
        chart.ChartAreas[0].AxisY2.MinorTickMark.LineColor = myForeColor;
        chart.ChartAreas[0].AxisX.MajorTickMark.LineColor = myForeColor;
        chart.ChartAreas[0].AxisX.MinorTickMark.LineColor = myForeColor;


        // Set color of the grid lines
        chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(120, 120, 120);
        chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(120, 120, 120);
        chart.ChartAreas[0].AxisY2.MajorGrid.LineColor = Color.FromArgb(120, 120, 120);

        // Set color of the legend
        chart.Legends[0].ForeColor = myForeColor;
        chart.Legends[0].BackColor = myBackgroundColor;

        chart.ChartAreas["ChartArea1"].AxisY2.TitleForeColor = myForeColor;
        chart.ChartAreas["ChartArea1"].AxisY.TitleForeColor = myForeColor;
    }

    /// <summary>
    /// Method for changing the application to day mode.
    /// </summary>
    private void DarkModeOff()
    {
        cData.DarkMode = DarkMode = false;
        Color myBackgroundColor = SystemColors.Control;
        Color myForeColor = SystemColors.ControlText;

        foreach (var item in screens.Values)         // going through all Forms (User Controls)
        {
            item.BackColor = SystemColors.Control;   // color of the form (User Control) background

            foreach (Control c in item.Controls)     // going through all Controls in particular Form (User Control)
            {
                if (c is Label)
                {
                    Label l = (Label)c;
                    l.ForeColor = myForeColor;
                    l.BackColor = myBackgroundColor;

                    if (TimeIsUp && l.Name == "lblAlarmTime")
                    {
                        l.ForeColor = Color.FromArgb(238, 70, 90);
                    }

                }
                else if (c is Button)
                {
                    Button b = (Button)c;
                    b.ForeColor = myForeColor;
                    b.BackColor = SystemColors.ButtonHighlight;

                    if (b.Enabled == false)
                    {
                        b.BackColor = SystemColors.ButtonFace;
                    }


                    if (TimeIsUp && (b.Name == "btnBreakYes" || b.Name == "btnBreakNo"))
                    {
                        b.ForeColor = SystemColors.ControlText;

                        if (b.Name == "btnBreakYes")
                            b.BackColor = Color.LightGreen;
                        if (b.Name == "btnBreakNo")
                            b.BackColor = Color.FromArgb(255, 153, 163); // Light pink  
                    }
                    if (TimeIsUp && PomoCounter == 4 && b.Name == "btnPomodoro")
                    {
                        b.BackColor = Color.FromArgb(238, 70, 90);
                    }


                }
                else if (c is ComboBox)
                {
                    ComboBox cb = (ComboBox)c;
                    cb.ForeColor = myForeColor;
                    cb.BackColor = Color.White;
                }
                else if (c is RichTextBox)
                {
                    RichTextBox r = (RichTextBox)c;
                    r.ForeColor = myForeColor;
                    r.BackColor = myBackgroundColor;
                    r.SelectionColor = myForeColor;
                }


                else if (c is Panel) // will work both for Panel and TableLayoutPanel
                {
                    DarkModeOffTLP((Panel)c);
                }
                else if (c is Chart)
                {
                    DarkModeOffCharts((Chart)c);
                }

            }
        }
    }



    /// <summary>
    /// Method for changing the application to day mode in TableLayoutPanel and Panel.
    /// </summary>
    /// <param name="tlp"></param>
    /// <param name="myForeColor2"></param>
    /// <param name="myBackgroundColor2"></param>
    private void DarkModeOffTLP(Panel tlp)
    {
        foreach (Control c2 in tlp.Controls)
        {
            // Warning: as LinkLabel is derived from Label, it must be checked first!
            if (c2 is LinkLabel)
            {
                LinkLabel ll = (LinkLabel)c2;
                ll.LinkColor = SystemColors.ControlText;
                ll.ActiveLinkColor = SystemColors.ControlText;
                ll.VisitedLinkColor = SystemColors.ControlText;
                ll.ForeColor = SystemColors.ControlText;
                ll.BackColor = SystemColors.Control;
            }
            else if (c2 is Label)
            {
                Label l = (Label)c2;
                l.ForeColor = SystemColors.ControlText;
                l.BackColor = SystemColors.Control;
            }
            else if (c2 is RadioButton)
            {
                RadioButton r = (RadioButton)c2;
                r.ForeColor = SystemColors.ControlText;
                r.BackColor = SystemColors.Control;
            }
            else if (c2 is CheckBox)
            {
                CheckBox ch = (CheckBox)c2;
                ch.ForeColor = SystemColors.ControlText;
                ch.BackColor = SystemColors.Control;
            }
            else if (c2 is NumericUpDown)
            {
                NumericUpDown n = (NumericUpDown)c2;
                n.ForeColor = SystemColors.ControlText;
                n.BackColor = SystemColors.Control;
            }

            else if (c2 is RichTextBox)
            {
                RichTextBox r = (RichTextBox)c2;
                r.ForeColor = SystemColors.ControlText;
                r.BackColor = SystemColors.Control;
                r.SelectionColor = SystemColors.ControlText;
            }
        }
    }

    private void DarkModeOffCharts(Chart chart)
    {
        // Set the chart background to the default light mode color
        chart.BackColor = SystemColors.Control;  //Color.White;
        chart.ChartAreas[0].BackColor = Color.White;

        // Set the axes line colors to the default light mode color
        chart.ChartAreas[0].AxisX.LineColor = Color.Black;
        chart.ChartAreas[0].AxisY.LineColor = Color.Black;
        chart.ChartAreas[0].AxisY2.LineColor = Color.Black;

        // Set the axes labels to the default light mode color
        chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.Black;
        chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.Black;
        chart.ChartAreas[0].AxisY2.LabelStyle.ForeColor = Color.Black;

        // Set the tick marks (next to the Y-axis labels) to the default light mode color
        chart.ChartAreas[0].AxisY.MajorTickMark.LineColor = Color.Black;
        chart.ChartAreas[0].AxisY.MinorTickMark.LineColor = Color.Black;
        chart.ChartAreas[0].AxisY2.MajorTickMark.LineColor = Color.Black;
        chart.ChartAreas[0].AxisY2.MinorTickMark.LineColor = Color.Black;
        chart.ChartAreas[0].AxisX.MajorTickMark.LineColor = Color.Black;
        chart.ChartAreas[0].AxisX.MinorTickMark.LineColor = Color.Black;

        // Set the grid lines to the default light mode color
        chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
        chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
        chart.ChartAreas[0].AxisY2.MajorGrid.LineColor = Color.LightGray;

        // Set the legend colors to the default light mode color
        chart.Legends[0].ForeColor = SystemColors.ControlText;
        chart.Legends[0].BackColor = SystemColors.Control;

        // Set the axis titles to the default light mode color
        chart.ChartAreas["ChartArea1"].AxisY2.TitleForeColor = Color.Black;
        chart.ChartAreas["ChartArea1"].AxisY.TitleForeColor = Color.Black;
    }


    // ************************************************************************************************
    // * Another methods
    // *


    /// <summary>
    /// Save settings and graph data to json file every 10 minutes
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tmrSaveData_Tick(object sender, EventArgs e)
    {
        // Save settings
        if (cData != null)
            ConfigManager.SaveJsonConfigFile(cData);
        else throw new Exception("Data is null!");

        // Also if the random color is selected, change the color every 10 minutes.
        if (((UcSettings)screens["btnSettings"]).rdoRandom.Checked)
        {
            cData.SelectedColor = ColorTranslator.ToHtml(((UcSettings)screens["btnSettings"]).RandomColor());
            SetStripsColor();
        }
    }

    /// <summary>
    /// Save settings and graph data to json files fefore the application is closed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (cData != null)
            ConfigManager.SaveJsonConfigFile(cData);

    }

  
}
