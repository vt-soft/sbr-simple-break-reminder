using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SBR;
using static System.Windows.Forms.LinkLabel;

namespace SBR.Forms
{
    public partial class FrmTranslators : Form
    {

        public FrmTranslators()
        {
            InitializeComponent();

            lblText.Text = "Is your language missing here? Are you a native speaker who wants to help?" +
                           "\nClick the link below to find out how you can get involved." +
                           "\nThere are only a few words to translate, so it will take you only a few minutes! :)";

            llbLink.Text = "https://www.vt-soft.com/looking-for-translators";


            pictureBox1.Image = SystemIcons.Question.ToBitmap();
       
        }

        /// <summary>
        /// Open the link in the default browser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void llbLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = llbLink.Text,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
