namespace SBR.Forms
{
    partial class UcAbout
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblAbout6 = new Label();
            pnlAppsContainer = new FlowLayoutPanel();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            richTextBoxAbout = new RichTextBox();
            SuspendLayout();
            // 
            // lblAbout6
            // 
            lblAbout6.Font = new Font("Segoe UI", 9.75F);
            lblAbout6.Location = new Point(502, 30);
            lblAbout6.Name = "lblAbout6";
            lblAbout6.Size = new Size(294, 42);
            lblAbout6.TabIndex = 1;
            lblAbout6.Text = "*Also check out these free apps:";
            lblAbout6.Visible = false;
            // 
            // pnlAppsContainer
            // 
            pnlAppsContainer.AutoScroll = true;
            pnlAppsContainer.Location = new Point(502, 78);
            pnlAppsContainer.Name = "pnlAppsContainer";
            pnlAppsContainer.Size = new Size(310, 398);
            pnlAppsContainer.TabIndex = 4;
            // 
            // richTextBoxAbout
            // 
            richTextBoxAbout.BorderStyle = BorderStyle.None;
            richTextBoxAbout.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 238);
            richTextBoxAbout.Location = new Point(22, 30);
            richTextBoxAbout.Name = "richTextBoxAbout";
            richTextBoxAbout.ReadOnly = true;
            richTextBoxAbout.Size = new Size(451, 438);
            richTextBoxAbout.TabIndex = 1;
            richTextBoxAbout.Text = "";
            // 
            // UcAbout
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(richTextBoxAbout);
            Controls.Add(pnlAppsContainer);
            Controls.Add(lblAbout6);
            Name = "UcAbout";
            Size = new Size(824, 509);
            ResumeLayout(false);
        }

        #endregion
        private Label lblAbout6;
        private FlowLayoutPanel pnlAppsContainer;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private RichTextBox richTextBoxAbout;
    }
}
