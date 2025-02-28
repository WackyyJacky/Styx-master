namespace Styx.UI
{
    partial class Sniffer
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
            this.txtPackets = new System.Windows.Forms.RichTextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.ClearPacketsBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtPackets
            // 
            this.txtPackets.Location = new System.Drawing.Point(32, 97);
            this.txtPackets.Margin = new System.Windows.Forms.Padding(8);
            this.txtPackets.Name = "txtPackets";
            this.txtPackets.Size = new System.Drawing.Size(1562, 541);
            this.txtPackets.TabIndex = 5;
            this.txtPackets.Text = "";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(968, 29);
            this.btnSave.Margin = new System.Windows.Forms.Padding(8);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(200, 54);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(1184, 29);
            this.btnStop.Margin = new System.Windows.Forms.Padding(8);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(200, 54);
            this.btnStop.TabIndex = 8;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(1400, 29);
            this.btnStart.Margin = new System.Windows.Forms.Padding(8);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(200, 54);
            this.btnStart.TabIndex = 9;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // ClearPacketsBtn
            // 
            this.ClearPacketsBtn.Location = new System.Drawing.Point(752, 29);
            this.ClearPacketsBtn.Margin = new System.Windows.Forms.Padding(8);
            this.ClearPacketsBtn.Name = "ClearPacketsBtn";
            this.ClearPacketsBtn.Size = new System.Drawing.Size(200, 54);
            this.ClearPacketsBtn.TabIndex = 10;
            this.ClearPacketsBtn.Text = "Clear";
            this.ClearPacketsBtn.UseVisualStyleBackColor = true;
            this.ClearPacketsBtn.Click += new System.EventHandler(this.ClearPacketsBtn_Click);
            // 
            // Sniffer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1632, 672);
            this.Controls.Add(this.ClearPacketsBtn);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtPackets);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(8);
            this.Name = "Sniffer";
            this.Text = "Packet Sniffer";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Sniffer_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.RichTextBox txtPackets;
        public System.Windows.Forms.Button btnSave;
        public System.Windows.Forms.Button btnStop;
        public System.Windows.Forms.Button btnStart;
        public System.Windows.Forms.Button ClearPacketsBtn;
    }
}