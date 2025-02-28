using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using Styx.Botting;
using Styx.Util;
using Application = UnityEngine.Application;

namespace Styx.UI
{
    public partial class Root : Form
    {
        public static readonly Root Instance = new Root();

        private readonly byte[] _link =
        {
            0x68, 0x74, 0x74, 0x70, 0x73, 0x3A, 0x2F, 0x2F, 0x64, 0x69, 0x73, 0x63, 0x6F, 0x72, 0x64, 0x2E, 0x67, 0x67,
            0x2F, 0x77, 0x71, 0x79, 0x52, 0x4D, 0x58, 0x4D
        };

        public Root()
        {
            InitializeComponent();
        }

        private void Root_Load(object sender, EventArgs e)
        {
            AdaptGameWindow();
            mainMenu.Parent = this;
        }

        private void Root_Resize(object sender, EventArgs e)
        {
            if (GameAttach.GameWindowAttached) GameAttach.Resize(ref pnlGame);
        }

        public void AdaptGameWindow()
        {
            GameAttach.GameWindowHandle = GameAttach.FindWindow(null, "AQ3D");
            if (GameAttach.GameWindowHandle == IntPtr.Zero)
            {
                System.Windows.Forms.MessageBox.Show("Unable to find AdventureQuest 3D!");
                Core.Loader.Unload();
            }

            GameAttach.OriginalWindowStyle = GameAttach.GetWindowLong(GameAttach.GameWindowHandle, GameAttach.GwlStyle);
            GameAttach.GetWindowRect(GameAttach.GameWindowHandle, out GameAttach.OriginalWindowPos);
            GameAttach.SetWindowLong(GameAttach.GameWindowHandle, GameAttach.GwlStyle, GameAttach.WsVisible);
            GameAttach.SetParent(GameAttach.GameWindowHandle, pnlGame.Handle);
            GameAttach.MoveWindow(GameAttach.GameWindowHandle, 0, 0, pnlGame.Width, pnlGame.Height, true);
            GameAttach.GameWindowAttached = true;
        }

        private void Root_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            // ResetUI();
            Core.Loader.Unload();
            Application.Quit();
        }

        public void ShowForm(Form form)
        {
            if (form.Visible) form.Hide();
            else if (AutoLogin.IsLoggedIn && AutoLogin.IsLoaded) form.Show();
        }

        private void botToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm(BotManager.Instance);
        }

        private void grabbersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm(Grabbers.Instance);
        }

        private void loadersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm(Loaders.Instance);
        }

        private void packetSpammerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm(Spammer.Instance);
        }

        private void packetSnifferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm(Sniffer.Instance);
        }

        private void hotkeysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm(KeyBinds.Instance);
        }

        private void onlineStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm(Staff.Instance);
        }

        private void copyLinkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText("https://discordapp.com/invite/HnqSWUa");
        }

        private void unloadStyxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetUi();
            Core.Loader.Unload();
        }

        public void ResetUi()
        {
            GameAttach.SetParent(GameAttach.GameWindowHandle, IntPtr.Zero);
            GameAttach.SetWindowLong(GameAttach.GameWindowHandle, GameAttach.GwlStyle,
                (int)GameAttach.OriginalWindowStyle);
            GameAttach.MoveWindow(GameAttach.GameWindowHandle, GameAttach.OriginalWindowPos.left,
                GameAttach.OriginalWindowPos.top, GameAttach.OriginalWindowPos.right,
                GameAttach.OriginalWindowPos.bottom, true);
        }

        public void SetStaffCount(int count)
        {
            onlineStripMenuItem.Text = $"Staff Nearby: {count}";
        }

        private void openLinkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("https://discordapp.com/invite/HnqSWUa");
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show(@"Something went wrong. Please notify us in our Discord server.", "Styx Error: ToolStrip");
            }
        }

        private void Root_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt)
            {
                mainMenu.Visible = !mainMenu.Visible;
                GameAttach.Resize(ref pnlGame);
            }
        }

        private void styxBotcomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("http://styx-bot.com");
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show(@"Something went wrong, this is probably due to a game update. Please notify us in our Discord server.", "Styx Error: Web");
            }
        }
    }
}