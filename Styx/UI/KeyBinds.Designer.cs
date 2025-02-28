namespace Styx.UI
{
    partial class KeyBinds
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
            this.cbKeys = new System.Windows.Forms.ComboBox();
            this.cbActions = new System.Windows.Forms.ComboBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.gbActive = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.lstActive = new System.Windows.Forms.ListBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.gbActive.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbKeys
            // 
            this.cbKeys.FormattingEnabled = true;
            this.cbKeys.Location = new System.Drawing.Point(32, 29);
            this.cbKeys.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.cbKeys.Name = "cbKeys";
            this.cbKeys.Size = new System.Drawing.Size(220, 39);
            this.cbKeys.TabIndex = 0;
            // 
            // cbActions
            // 
            this.cbActions.FormattingEnabled = true;
            this.cbActions.Items.AddRange(new object[] {
            "Toggle Bot Manager window",
            "Toggle Data Grabbers window",
            "Toggle Loaders window",
            "Toggle Packet Spammer window",
            "Toggle Packet Sniffer window",
            "Toggle Hotkeys window",
            "Toggle Bank",
            "Teleport to cursor position",
            "Toggle jump hack",
            "Toggle Bot On/Off",
            "Toggle fly cheat"});
            this.cbActions.Location = new System.Drawing.Point(275, 29);
            this.cbActions.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.cbActions.Name = "cbActions";
            this.cbActions.Size = new System.Drawing.Size(415, 39);
            this.cbActions.TabIndex = 1;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(496, 93);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(200, 55);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // gbActive
            // 
            this.gbActive.Controls.Add(this.button1);
            this.gbActive.Controls.Add(this.btnSave);
            this.gbActive.Controls.Add(this.lstActive);
            this.gbActive.Controls.Add(this.btnRemove);
            this.gbActive.Location = new System.Drawing.Point(32, 162);
            this.gbActive.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.gbActive.Name = "gbActive";
            this.gbActive.Padding = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.gbActive.Size = new System.Drawing.Size(664, 329);
            this.gbActive.TabIndex = 3;
            this.gbActive.TabStop = false;
            this.gbActive.Text = "Active Hotkeys";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(16, 255);
            this.button1.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(200, 55);
            this.button1.TabIndex = 3;
            this.button1.Text = "Load";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(232, 255);
            this.btnSave.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(200, 55);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lstActive
            // 
            this.lstActive.FormattingEnabled = true;
            this.lstActive.ItemHeight = 31;
            this.lstActive.Location = new System.Drawing.Point(16, 45);
            this.lstActive.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.lstActive.Name = "lstActive";
            this.lstActive.ScrollAlwaysVisible = true;
            this.lstActive.Size = new System.Drawing.Size(625, 190);
            this.lstActive.TabIndex = 1;
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(448, 255);
            this.btnRemove.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(200, 55);
            this.btnRemove.TabIndex = 0;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // KeyBinds
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(725, 515);
            this.Controls.Add(this.gbActive);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.cbActions);
            this.Controls.Add(this.cbKeys);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Name = "KeyBinds";
            this.Text = "Hotkeys";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Hotkeys_FormClosing);
            this.Load += new System.EventHandler(this.Hotkeys_Load);
            this.gbActive.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ComboBox cbKeys;
        public System.Windows.Forms.ComboBox cbActions;
        public System.Windows.Forms.Button btnAdd;
        public System.Windows.Forms.GroupBox gbActive;
        public System.Windows.Forms.ListBox lstActive;
        public System.Windows.Forms.Button btnRemove;
        public System.Windows.Forms.Button btnSave;
        public System.Windows.Forms.Button button1;
    }
}