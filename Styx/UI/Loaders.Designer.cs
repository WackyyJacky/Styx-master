namespace Styx.UI
{
    partial class Loaders
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
            this.btnQuest = new System.Windows.Forms.Button();
            this.btnShop = new System.Windows.Forms.Button();
            this.btnMerge = new System.Windows.Forms.Button();
            this.numId = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numId)).BeginInit();
            this.SuspendLayout();
            // 
            // btnQuest
            // 
            this.btnQuest.Location = new System.Drawing.Point(26, 211);
            this.btnQuest.Margin = new System.Windows.Forms.Padding(8);
            this.btnQuest.Name = "btnQuest";
            this.btnQuest.Size = new System.Drawing.Size(308, 54);
            this.btnQuest.TabIndex = 5;
            this.btnQuest.Text = "Quest";
            this.btnQuest.UseVisualStyleBackColor = true;
            this.btnQuest.Click += new System.EventHandler(this.btnQuest_Click);
            // 
            // btnShop
            // 
            this.btnShop.Location = new System.Drawing.Point(26, 72);
            this.btnShop.Margin = new System.Windows.Forms.Padding(8);
            this.btnShop.Name = "btnShop";
            this.btnShop.Size = new System.Drawing.Size(308, 54);
            this.btnShop.TabIndex = 6;
            this.btnShop.Text = "Shop";
            this.btnShop.UseVisualStyleBackColor = true;
            this.btnShop.Click += new System.EventHandler(this.btnShop_Click);
            // 
            // btnMerge
            // 
            this.btnMerge.Location = new System.Drawing.Point(26, 141);
            this.btnMerge.Margin = new System.Windows.Forms.Padding(8);
            this.btnMerge.Name = "btnMerge";
            this.btnMerge.Size = new System.Drawing.Size(308, 54);
            this.btnMerge.TabIndex = 7;
            this.btnMerge.Text = "Merge Shop";
            this.btnMerge.UseVisualStyleBackColor = true;
            this.btnMerge.Click += new System.EventHandler(this.btnMerge_Click);
            // 
            // numId
            // 
            this.numId.Location = new System.Drawing.Point(84, 14);
            this.numId.Margin = new System.Windows.Forms.Padding(8);
            this.numId.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numId.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numId.Name = "numId";
            this.numId.Size = new System.Drawing.Size(250, 38);
            this.numId.TabIndex = 8;
            this.numId.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 17);
            this.label1.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 32);
            this.label1.TabIndex = 9;
            this.label1.Text = "ID:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(26, 281);
            this.button1.Margin = new System.Windows.Forms.Padding(8);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(308, 54);
            this.button1.TabIndex = 10;
            this.button1.Text = "Map";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Loaders
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 355);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numId);
            this.Controls.Add(this.btnMerge);
            this.Controls.Add(this.btnShop);
            this.Controls.Add(this.btnQuest);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(8);
            this.Name = "Loaders";
            this.Text = "Loaders";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Loaders_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.numId)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button btnQuest;
        public System.Windows.Forms.Button btnShop;
        public System.Windows.Forms.Button btnMerge;
        public System.Windows.Forms.NumericUpDown numId;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Button button1;
    }
}