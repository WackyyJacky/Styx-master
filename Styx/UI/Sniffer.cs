using System;
using System.IO;
using System.Windows.Forms;
using PacketSniffer = Styx.Tools.Packets.Sniffer;

namespace Styx.UI
{
    public partial class Sniffer : Form
    {
        private Sniffer()
        {
            InitializeComponent();
        }

        public static Sniffer Instance { get; } = new Sniffer();

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Save captured packets";
                ofd.CheckFileExists = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                    try
                    {
                        File.WriteAllText(ofd.FileName, txtPackets.Text);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show($"File write operation failed: \n{ex.Message}", "Styx",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            PacketSniffer.Instance.Stop();
            PacketSniffer.Instance.PacketReceived -= OnPacketReceived;
            PacketSniffer.Instance.PacketSent -= OnPacketSent;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            PacketSniffer.Instance.Start();
            PacketSniffer.Instance.PacketReceived += OnPacketReceived;
            PacketSniffer.Instance.PacketSent += OnPacketSent;
        }

        private void OnPacketReceived(string packet)
        {
            txtPackets.Invoke(new MethodInvoker(() =>
            {
                if (!packet.Contains("\"type\":17,\"cmd\":19"))
                    txtPackets.AppendText("[RECEIVED] " + packet + System.Environment.NewLine);
            }));
        }

        private void OnPacketSent(string packet)
        {
            txtPackets.Invoke(new MethodInvoker(() =>
            {
                txtPackets.AppendText("[SENT] " + packet + System.Environment.NewLine);
            }));
        }

        private void Sniffer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void ClearPacketsBtn_Click(object sender, EventArgs e)
        {
            txtPackets.Clear();
        }
    }
}