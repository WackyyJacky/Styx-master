namespace Styx.UI
{
    partial class Spammer
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
            this.lstPackets = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.numDelay = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.txtPacket = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.numDelay)).BeginInit();
            this.SuspendLayout();
            // 
            // lstPackets
            // 
            this.lstPackets.FormattingEnabled = true;
            this.lstPackets.ItemHeight = 31;
            this.lstPackets.Location = new System.Drawing.Point(32, 124);
            this.lstPackets.Margin = new System.Windows.Forms.Padding(8);
            this.lstPackets.Name = "lstPackets";
            this.lstPackets.ScrollAlwaysVisible = true;
            this.lstPackets.Size = new System.Drawing.Size(1172, 345);
            this.lstPackets.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 21);
            this.label1.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 32);
            this.label1.TabIndex = 11;
            this.label1.Text = "Packet";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(992, 54);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(8);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(102, 54);
            this.btnAdd.TabIndex = 13;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(1010, 506);
            this.btnStart.Margin = new System.Windows.Forms.Padding(8);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(200, 54);
            this.btnStart.TabIndex = 14;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(794, 506);
            this.btnStop.Margin = new System.Windows.Forms.Padding(8);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(200, 54);
            this.btnStop.TabIndex = 15;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(578, 506);
            this.btnRemove.Margin = new System.Windows.Forms.Padding(8);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(200, 54);
            this.btnRemove.TabIndex = 16;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(362, 506);
            this.btnClear.Margin = new System.Windows.Forms.Padding(8);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(200, 54);
            this.btnClear.TabIndex = 17;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // numDelay
            // 
            this.numDelay.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numDelay.Location = new System.Drawing.Point(200, 508);
            this.numDelay.Margin = new System.Windows.Forms.Padding(8);
            this.numDelay.Maximum = new decimal(new int[] {
            61000,
            0,
            0,
            0});
            this.numDelay.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numDelay.Name = "numDelay";
            this.numDelay.Size = new System.Drawing.Size(146, 38);
            this.numDelay.TabIndex = 18;
            this.numDelay.Value = new decimal(new int[] {
            2500,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(94, 513);
            this.label2.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 32);
            this.label2.TabIndex = 19;
            this.label2.Text = "Delay";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1106, 54);
            this.button1.Margin = new System.Windows.Forms.Padding(8);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(102, 54);
            this.button1.TabIndex = 20;
            this.button1.Text = "Send";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtPacket
            // 
            this.txtPacket.Location = new System.Drawing.Point(32, 60);
            this.txtPacket.Margin = new System.Windows.Forms.Padding(8);
            this.txtPacket.Name = "txtPacket";
            this.txtPacket.Size = new System.Drawing.Size(940, 38);
            this.txtPacket.TabIndex = 12;
            // 
            // Spammer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1242, 589);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numDelay);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.txtPacket);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lstPackets);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(8);
            this.Name = "Spammer";
            this.Text = "Packet Spammer";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Spammer_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.numDelay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ListBox lstPackets;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Button btnAdd;
        public System.Windows.Forms.Button btnStart;
        public System.Windows.Forms.Button btnStop;
        public System.Windows.Forms.Button btnRemove;
        public System.Windows.Forms.Button btnClear;
        public System.Windows.Forms.NumericUpDown numDelay;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.Button button1;
        public System.Windows.Forms.TextBox txtPacket;
    }
}