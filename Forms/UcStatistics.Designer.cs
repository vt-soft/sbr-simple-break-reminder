namespace SBR.Forms
{
    partial class UcStatistics
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
            components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            lblStats = new Label();
            chartMonths = new System.Windows.Forms.DataVisualization.Charting.Chart();
            tmrMonthChart = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)chartMonths).BeginInit();
            SuspendLayout();
            // 
            // lblStats
            // 
            lblStats.AutoSize = true;
            lblStats.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 238);
            lblStats.Location = new Point(30, 20);
            lblStats.Name = "lblStats";
            lblStats.Size = new Size(278, 21);
            lblStats.TabIndex = 1;
            lblStats.Text = "*Daily averages for the last 14 months:";
            // 
            // chartMonths
            // 
            chartArea1.BackColor = Color.White;
            chartArea1.Name = "ChartArea1";
            chartMonths.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            chartMonths.Legends.Add(legend1);
            chartMonths.Location = new Point(7, 66);
            chartMonths.Name = "chartMonths";
            series1.ChartArea = "ChartArea1";
            series1.Color = Color.Red;
            series1.Legend = "Legend1";
            series1.Name = "Ignored breaks";
            series1.YValuesPerPoint = 2;
            series2.ChartArea = "ChartArea1";
            series2.Color = Color.DodgerBlue;
            series2.CustomProperties = "DrawSideBySide=True";
            series2.Legend = "Legend1";
            series2.Name = "Working time";
            series2.YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
            series2.YValuesPerPoint = 2;
            series3.ChartArea = "ChartArea1";
            series3.Color = Color.DarkKhaki;
            series3.Legend = "Legend1";
            series3.Name = "Total time";
            series3.YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
            chartMonths.Series.Add(series1);
            chartMonths.Series.Add(series2);
            chartMonths.Series.Add(series3);
            chartMonths.Size = new Size(803, 398);
            chartMonths.TabIndex = 40;
            chartMonths.Text = "chart1";
            chartMonths.Enter += chartMonths_Enter;
            // 
            // tmrMonthChart
            // 
            tmrMonthChart.Interval = 10000;
            // 
            // UcStatistics
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(chartMonths);
            Controls.Add(lblStats);
            Name = "UcStatistics";
            Size = new Size(824, 509);
            ((System.ComponentModel.ISupportInitialize)chartMonths).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label lblStats;
        public System.Windows.Forms.DataVisualization.Charting.Chart chartMonths;
        private System.Windows.Forms.Timer tmrMonthChart;
    }
}
