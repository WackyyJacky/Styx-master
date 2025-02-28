using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using PacketSpammer = Styx.Tools.Packets.Spammer;

namespace Styx.UI
{
    public partial class Spammer : Form
    {
        private List<object[]> _requestParams;
        private MethodInfo _sendMessage;

        private Spammer()
        {
            InitializeComponent();
        }

        public static Spammer Instance { get; } = new Spammer();

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtPacket.Text.Length > 0)
            {
                lstPackets.Items.Add(txtPacket.Text);
                txtPacket.Clear();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lstPackets.Items.Clear();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            var index = lstPackets.SelectedIndex;

            if (index >= 0) lstPackets.Items.RemoveAt(index);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            PacketSpammer.Instance.Stop();
            PacketSpammer.Instance.IndexChanged -= UpdateIndex;
            btnStart.Enabled = true;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (lstPackets.Items.Count > 0)
            {
                var reqs = lstPackets.Items.Cast<string>().ToList();
                PacketSpammer.Instance.IndexChanged += UpdateIndex;
                PacketSpammer.Instance.Start(reqs, (int) numDelay.Value);
                btnStart.Enabled = false;
            }
        }

        private void UpdateIndex(int index)
        {
            lstPackets.SelectedIndex = index;
        }

        private void Spammer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _requestParams = new List<object[]>();
            var req = JsonConvert.DeserializeObject<Request>(txtPacket.Text);
            _requestParams.Add(new object[] {Encoding.ASCII.GetBytes(txtPacket.Text), req.type, req.cmd});

            _sendMessage = Game.Instance.aec.GetType()
                .GetMethod("sendMessage", BindingFlags.NonPublic | BindingFlags.Instance);
            _sendMessage.Invoke(Game.Instance.aec, _requestParams[0]);
        }
    }
}