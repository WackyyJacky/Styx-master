namespace Styx.UI
{
    partial class Staff
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lstStaff = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // lstStaff
            // 
            this.lstStaff.FormattingEnabled = true;
            this.lstStaff.ItemHeight = 31;
            this.lstStaff.Location = new System.Drawing.Point(32, 29);
            this.lstStaff.Margin = new System.Windows.Forms.Padding(8);
            this.lstStaff.Name = "lstStaff";
            this.lstStaff.Size = new System.Drawing.Size(548, 283);
            this.lstStaff.TabIndex = 0;
            // 
            // Staff
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(618, 341);
            this.Controls.Add(this.lstStaff);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(8);
            this.Name = "Staff";
            this.Text = "Staff Nearby";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Staff_FormClosing);
            this.Load += new System.EventHandler(this.Staff_Load);
            this.VisibleChanged += new System.EventHandler(this.Staff_VisibleChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lstStaff;
    }
}