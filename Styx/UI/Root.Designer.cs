namespace Styx.UI
{
    partial class Root
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Root));
            this.pnlGame = new System.Windows.Forms.Panel();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.botToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grabbersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.packetSpammerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.packetSnifferToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hotkeysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onlineStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.discordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.styxBotcomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openLinkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyLinkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unloadStyxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlGame
            // 
            this.pnlGame.AutoSize = true;
            this.pnlGame.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlGame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGame.Location = new System.Drawing.Point(0, 58);
            this.pnlGame.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.pnlGame.Name = "pnlGame";
            this.pnlGame.Size = new System.Drawing.Size(1643, 786);
            this.pnlGame.TabIndex = 0;
            // 
            // mainMenu
            // 
            this.mainMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.botToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.onlineStripMenuItem,
            this.discordToolStripMenuItem,
            this.unloadStyxToolStripMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Padding = new System.Windows.Forms.Padding(16, 5, 0, 5);
            this.mainMenu.Size = new System.Drawing.Size(1643, 58);
            this.mainMenu.TabIndex = 3;
            this.mainMenu.Text = "menuStrip1";
            // 
            // botToolStripMenuItem
            // 
            this.botToolStripMenuItem.Name = "botToolStripMenuItem";
            this.botToolStripMenuItem.Size = new System.Drawing.Size(201, 48);
            this.botToolStripMenuItem.Text = "Bot Manager";
            this.botToolStripMenuItem.Click += new System.EventHandler(this.botToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadersToolStripMenuItem,
            this.grabbersToolStripMenuItem,
            this.packetSpammerToolStripMenuItem,
            this.packetSnifferToolStripMenuItem,
            this.hotkeysToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(99, 48);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // loadersToolStripMenuItem
            // 
            this.loadersToolStripMenuItem.Name = "loadersToolStripMenuItem";
            this.loadersToolStripMenuItem.Size = new System.Drawing.Size(396, 46);
            this.loadersToolStripMenuItem.Text = "Loaders";
            this.loadersToolStripMenuItem.Click += new System.EventHandler(this.loadersToolStripMenuItem_Click);
            // 
            // grabbersToolStripMenuItem
            // 
            this.grabbersToolStripMenuItem.Name = "grabbersToolStripMenuItem";
            this.grabbersToolStripMenuItem.Size = new System.Drawing.Size(396, 46);
            this.grabbersToolStripMenuItem.Text = "Data Grabbers";
            this.grabbersToolStripMenuItem.Click += new System.EventHandler(this.grabbersToolStripMenuItem_Click);
            // 
            // packetSpammerToolStripMenuItem
            // 
            this.packetSpammerToolStripMenuItem.Name = "packetSpammerToolStripMenuItem";
            this.packetSpammerToolStripMenuItem.Size = new System.Drawing.Size(396, 46);
            this.packetSpammerToolStripMenuItem.Text = "Packet Spammer";
            this.packetSpammerToolStripMenuItem.Click += new System.EventHandler(this.packetSpammerToolStripMenuItem_Click);
            // 
            // packetSnifferToolStripMenuItem
            // 
            this.packetSnifferToolStripMenuItem.Name = "packetSnifferToolStripMenuItem";
            this.packetSnifferToolStripMenuItem.Size = new System.Drawing.Size(396, 46);
            this.packetSnifferToolStripMenuItem.Text = "Packet Sniffer";
            this.packetSnifferToolStripMenuItem.Click += new System.EventHandler(this.packetSnifferToolStripMenuItem_Click);
            // 
            // hotkeysToolStripMenuItem
            // 
            this.hotkeysToolStripMenuItem.Name = "hotkeysToolStripMenuItem";
            this.hotkeysToolStripMenuItem.Size = new System.Drawing.Size(396, 46);
            this.hotkeysToolStripMenuItem.Text = "Hotkeys";
            this.hotkeysToolStripMenuItem.Click += new System.EventHandler(this.hotkeysToolStripMenuItem_Click);
            // 
            // onlineStripMenuItem
            // 
            this.onlineStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.onlineStripMenuItem.Name = "onlineStripMenuItem";
            this.onlineStripMenuItem.Size = new System.Drawing.Size(223, 48);
            this.onlineStripMenuItem.Text = "Staff Nearby: 0";
            this.onlineStripMenuItem.Click += new System.EventHandler(this.onlineStripMenuItem_Click);
            // 
            // discordToolStripMenuItem
            // 
            this.discordToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.styxBotcomToolStripMenuItem,
            this.openLinkToolStripMenuItem,
            this.copyLinkToolStripMenuItem});
            this.discordToolStripMenuItem.Name = "discordToolStripMenuItem";
            this.discordToolStripMenuItem.Size = new System.Drawing.Size(96, 48);
            this.discordToolStripMenuItem.Text = "Links";
            // 
            // styxBotcomToolStripMenuItem
            // 
            this.styxBotcomToolStripMenuItem.Name = "styxBotcomToolStripMenuItem";
            this.styxBotcomToolStripMenuItem.Size = new System.Drawing.Size(376, 46);
            this.styxBotcomToolStripMenuItem.Text = "Styx-Bot.com";
            this.styxBotcomToolStripMenuItem.Click += new System.EventHandler(this.styxBotcomToolStripMenuItem_Click);
            // 
            // openLinkToolStripMenuItem
            // 
            this.openLinkToolStripMenuItem.Name = "openLinkToolStripMenuItem";
            this.openLinkToolStripMenuItem.Size = new System.Drawing.Size(376, 46);
            this.openLinkToolStripMenuItem.Text = "Open Discord Link";
            this.openLinkToolStripMenuItem.Click += new System.EventHandler(this.openLinkToolStripMenuItem_Click);
            // 
            // copyLinkToolStripMenuItem
            // 
            this.copyLinkToolStripMenuItem.Name = "copyLinkToolStripMenuItem";
            this.copyLinkToolStripMenuItem.Size = new System.Drawing.Size(376, 46);
            this.copyLinkToolStripMenuItem.Text = "Copy Discord Link";
            this.copyLinkToolStripMenuItem.Click += new System.EventHandler(this.copyLinkToolStripMenuItem_Click);
            // 
            // unloadStyxToolStripMenuItem
            // 
            this.unloadStyxToolStripMenuItem.Name = "unloadStyxToolStripMenuItem";
            this.unloadStyxToolStripMenuItem.Size = new System.Drawing.Size(188, 48);
            this.unloadStyxToolStripMenuItem.Text = "Unload Styx";
            this.unloadStyxToolStripMenuItem.Click += new System.EventHandler(this.unloadStyxToolStripMenuItem_Click);
            // 
            // Root
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1643, 844);
            this.Controls.Add(this.pnlGame);
            this.Controls.Add(this.mainMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Name = "Root";
            this.Text = "Styx 1.1.1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Root_FormClosing);
            this.Load += new System.EventHandler(this.Root_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Root_KeyDown);
            this.Resize += new System.EventHandler(this.Root_Resize);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStripMenuItem botToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem grabbersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem packetSpammerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem packetSnifferToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hotkeysToolStripMenuItem;
        public System.Windows.Forms.MenuStrip mainMenu;
        public System.Windows.Forms.Panel pnlGame;
        private System.Windows.Forms.ToolStripMenuItem onlineStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unloadStyxToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem discordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyLinkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openLinkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem styxBotcomToolStripMenuItem;
    }
}