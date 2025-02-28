using System;
using System.Windows.Forms;
using Styx.Tools;

namespace Styx.UI
{
    public partial class Staff : Form
    {
        public Staff()
        {
            InitializeComponent();
        }

        public static Staff Instance { get; } = new Staff();

        private void Staff_Load(object sender, EventArgs e)
        {
            GetStaff();
        }

        private void Staff_VisibleChanged(object sender, EventArgs e)
        {
            GetStaff();
        }

        private void Staff_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void GetStaff()
        {
            lstStaff.Items.Clear();
            var staff = World.Staff;
            if (staff?.Count > 0)
                foreach (var s in staff)
                    lstStaff.Items.Add($"{s.name} - {GetAccessTitle(s.AccessLevel)}");
        }

        private string GetAccessTitle(int accessLevel)
        {
            if (accessLevel >= 101) return "Super Administrator";
            if (accessLevel >= 100) return "Administrator";
            if (accessLevel >= 60) return "Moderator";
            if (accessLevel >= 55) return "White Hat";
            if (accessLevel >= 50) return "Tester";
            return "Unknown";
        }
    }
}