using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Styx.Botting;

namespace Styx.UI
{
    public partial class Loaders : Form
    {
        private Loaders()
        {
            InitializeComponent();
        }

        public static Loaders Instance { get; } = new Loaders();

        private void btnQuest_Click(object sender, EventArgs e)
        {
            var q = new List<int>(1)
            {
                (int) numId.Value
            };
            AEC.getInstance().ResponseReceived += OnQuestLoaded;
            // Game.Instance.SendQuestLoadRequest(q);
            AEC.getInstance().sendRequest(new RequestQuestLoad
            {
                QuestIDs = q
            });
        }

        private void OnQuestLoaded(Response r)
        {
            //private void ShowDetail(Quest quest)
            if (r.type == (byte) Com.Type.Quest && r.cmd == (byte) Com.CmdQuest.Load)
            {
                AEC.getInstance().ResponseReceived -= OnQuestLoaded;

                var q = new List<int>(1)
                {
                    ((ResponseQuestLoad) r).Quests.ElementAt(0).Value.ID
                };
                UIQuest.ShowQuests(q, q);
            }
        }

        private void btnShop_Click(object sender, EventArgs e)
        {
            UIShop.LoadShop((int) numId.Value);
        }

        private void btnMerge_Click(object sender, EventArgs e)
        {
            UIMerge.Load((int) numId.Value);
        }

        private void Loaders_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (AutoLogin.IsLoaded)
                AEC.getInstance().sendRequest(new RequestAreaJoin
                {
                    areaID = (int) numId.Value,
                    IsPrivate = false
                });
        }
    }
}