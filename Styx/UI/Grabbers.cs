using System;
using System.IO;
using System.Windows.Forms;
using Styx.Tools;

namespace Styx.UI
{
    public partial class Grabbers : Form
    {
        private Grabbers()
        {
            InitializeComponent();
        }

        public static Grabbers Instance { get; } = new Grabbers();

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtResults.Clear();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (txtResults.Text.Length > 0)
                Clipboard.SetText(txtResults.Text);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Save grabber results";
                ofd.CheckFileExists = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                    try
                    {
                        File.WriteAllText(ofd.FileName, txtResults.Text);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show(
                            $"File write operation failed: {ex.Message}", "Styx",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
            }
        }

        private void btnGrab_Click(object sender, EventArgs e)
        {
            try
            {
                var i = cbGrabbers.SelectedIndex;
                if (i > -1)
                {
                    var g = new Grabber();

                    switch (i)
                    {
                        case 0:
                            txtResults.Text = g.GrabInventory();
                            break;
                        case 1:
                            txtResults.Text = g.GrabShops();
                            break;
                        case 2:
                            txtResults.Text = g.GrabQuests();
                            break;
                        case 3:
                            txtResults.Text = g.GrabMonsters();
                            break;
                        case 4:
                            txtResults.Text = g.GrabMergeShops();
                            break;
                        case 5:
                            txtResults.Text = g.GrabMachines();
                            break;
                        default: return;
                    }
                }
            }
            catch (Exception ex)
            {
                txtResults.AppendText(ex.Message + " " + ex.StackTrace);
            }
        }

        private void Grabbers_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }
    }
}