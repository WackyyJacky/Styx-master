using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using Styx.Botting;
using Styx.Botting.Commands;
using Styx.Botting.Commands.Combat;
using Styx.Botting.Commands.Item;
using Styx.Botting.Commands.Map;
using Styx.Botting.Commands.Misc;
using Styx.Botting.Commands.Quest;
using Styx.Tools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Styx.UI
{
    public partial class BotManager : Form
    {
        private static readonly Dictionary<string, string> DefaultText = new Dictionary<string, string>
        {
            {nameof(txtKillFLoot), "Loot/objective name"},
            {nameof(txtKillFQty), "Quantity (* = any)"},
            {nameof(txtKillFMon), "Monster (* = any)"},
            {nameof(txtWaypoint), "Waypoint name"},
            {nameof(txtMoveNPC), "Monster/NPC"},
            {nameof(txtLoot), "Item name"}
        };

        public static GameObject ActiveBot;

        private BotManager()
        {
            InitializeComponent();
        }

        public static BotManager Instance { get; } = new BotManager();

        private ListBox SelectedList
        {
            get
            {
                switch (cbLists.SelectedIndex)
                {
                    case 0:
                        return lstCommands;
                    case 1:
                        return lstSpells;
                    case 2:
                        return lstLoot;
                    case 3:
                        return lstQuests;
                    default: return lstCommands;
                }
            }
        }

        private void TextBoxEnter(object sender, EventArgs e)
        {
            var t = (TextBox)sender;
            if (t.Text == DefaultText[t.Name]) t.Clear();
        }

        private void TextBoxLeave(object sender, EventArgs e)
        {
            var t = (TextBox)sender;
            if (string.IsNullOrEmpty(t.Text))
                t.Text = DefaultText[t.Name];
        }

        private void BotManager_Load(object sender, EventArgs e)
        {
            cbLists.SelectedIndex = 0;
            lstCommands.DisplayMember = "Text";
            lstSpells.DisplayMember = "Text";
            cbServers.Items.AddRange(ServerInfo.Servers.Where(s => s.State).Select(s => s.Name).ToArray());
        }

        private void BotManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void StaffDisable_CheckedChanged(object sender, EventArgs e)
        {
            if (StaffDisable.Checked)
            {
                var staff = World.Staff;
                if (staff != null)
                    if (staff.Count > 0)
                        if (chkStart.Checked || ActiveBot != null)
                        {
                            Bot.Instance.IsRunning = false;
                            Bot.Instance.IndexChanged -= OnIndexChanged;
                            Bot.Instance.IsRunningChanged -= OnIsRunningChanged;
                            Object.Destroy(ActiveBot);
                            ActiveBot = null;
                            if (checkBox1.Checked) ChangeQuality(5, true);
                            chkStart.Checked = false;
                        }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var shopId = decimal.ToInt32(numericUpDown2.Value);
            var mergeId = decimal.ToInt32(numericUpDown1.Value);

            lstCommands.Items.Add(new CommandMerge
            {
                Text = $"Craft Item: {decimal.ToInt32(numericUpDown1.Value)}",
                ShopId = shopId,
                MergeId = mergeId
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var packet = textBox1.Text;

            lstCommands.Items.Add(new CommandSendPack
            {
                Text = $"Send Packet: {textBox1.Text}",
                Packet = packet
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var itemQty = (short)numericUpDown5.Value;
            var shopId = (int)numericUpDown4.Value;
            var itemId = (int)numericUpDown3.Value;

            lstCommands.Items.Add(new CommandBuy
            {
                Text = $"Buy {itemId} x{itemQty}",
                ShopId = shopId,
                ItemId = itemId,
                ItemQty = itemQty
            });
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var shopId = decimal.ToInt32(numericUpDown2.Value);
            var mergeId = decimal.ToInt32(numericUpDown1.Value);

            lstCommands.Items.Add(new CommandClaimMerge
            {
                Text = $"Claim Item: {decimal.ToInt32(numericUpDown1.Value)}",
                MergeId = mergeId
            });
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (ActiveBot != null)
            {
                chkStart.Checked = false;
                Bot.Instance.IsRunning = false;
                Bot.Instance.IndexChanged -= OnIndexChanged;
                Bot.Instance.IsRunningChanged -= OnIsRunningChanged;
                Object.Destroy(ActiveBot);
                ActiveBot = null;
                if (checkBox1.Checked) ChangeQuality(5, true);
                button6.Enabled = false;
            }
            else
            {
                button6.Enabled = false;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            // DISABLE TIMER
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var priv = chkJoinPrivate.Checked;
            lstCommands.Items.Add(new CommandJoin
            {
                Text = $"Join Map: {numericUpDown7.Value} [{(priv ? "Private" : "Public")}]",
                MapId = (int)numericUpDown7.Value,
                RequestPrivateInstance = priv
            });
        }

        private void button7_Click(object sender, EventArgs e)
        {
            lstCommands.Items.Add(new CommandKillAll
            {
                Teleport = chkKillFTeleport.Checked,
                Text = $"Kill All [{(chkKillFTeleport.Checked ? "Teleport" : "Walk")}]",
                Skip = chkNotFoundSkip.Checked
            });
        }

        private void button8_Click(object sender, EventArgs e)
        {
            lstCommands.Items.Add(new CommandRestTeleport
            {
                Position = Entities.GetInstance().me.wrapper.transform.position
            });
        }

        private void button9_Click(object sender, EventArgs e)
        {
            var pos = Entities.GetInstance().me.wrapper.transform.position;
            lstCommands.Items.Add(new CommandKillRadius
            {
                Radius = (float)numericUpDown10.Value,
                Teleport = chkKillFTeleport.Checked,
                Position = pos,
                Text =
                    $"Kill Radius: {(float)numericUpDown10.Value} [{(chkKillFTeleport.Checked ? "Teleport" : "Walk")}]"
            });
        }

        private void button10_Click(object sender, EventArgs e)
        {
            string item;

            if ((item = textBox3.Text).Length > 0)
                lstCommands.Items.Add(new CommandRestEquip
                {
                    Text = $"Equip: {item}",
                    Item = item
                });
        }

        private void button11_Click(object sender, EventArgs e)
        {
            string item;

            if ((item = textBox3.Text).Length > 0)
                lstCommands.Items.Add(new CommandUseItem
                {
                    Text = $"Use: {item}",
                    Item = item
                });
        }

        private void LstCommands_OnDoubleClick(object sender, EventArgs e)
        {
            if (lstCommands.SelectedItem is CommandKill)
            {
                var newEditedCommand = (CommandKill)lstCommands.SelectedItem;
                txtKillFMon.Text = newEditedCommand.Monster;
                chkKillFTeleport.Checked = newEditedCommand.Teleport;
                KillAmount.Value = newEditedCommand.Amount;
                chkNotFoundSkip.Checked = newEditedCommand.Skip;
            }
            else if (lstCommands.SelectedItem is CommandKillAll)
            {
                var newEditedCommand = (CommandKillAll)lstCommands.SelectedItem;
                chkKillFTeleport.Checked = newEditedCommand.Teleport;
                chkNotFoundSkip.Checked = newEditedCommand.Skip;
            }
            else if (lstCommands.SelectedItem is CommandKillAllFor)
            {
                var newEditedCommand = (CommandKillAllFor)lstCommands.SelectedItem;
                chkKillFTeleport.Checked = newEditedCommand.Teleport;
                if (newEditedCommand.IsLoot)
                    rbLoot.TabStop = newEditedCommand.IsLoot;
                else
                    rbQuestObj.TabStop = !newEditedCommand.IsLoot;

                txtKillFMon.Text = newEditedCommand.Monster;
                txtKillFLoot.Text = newEditedCommand.ItemName;
                txtKillFQty.Text = newEditedCommand.ItemQuantity;
                chkKillFTeleport.Checked = newEditedCommand.Teleport;
                checkStopItem.Checked = newEditedCommand.ItemStop;
                chkNotFoundSkip.Checked = newEditedCommand.Skip;
            }
            else if (lstCommands.SelectedItem is CommandKillFor)
            {
                var newEditedCommand = (CommandKillAllFor)lstCommands.SelectedItem;
                chkKillFTeleport.Checked = newEditedCommand.Teleport;
                if (newEditedCommand.IsLoot)
                    rbLoot.TabStop = newEditedCommand.IsLoot;
                else
                    rbQuestObj.TabStop = !newEditedCommand.IsLoot;

                txtKillFMon.Text = newEditedCommand.Monster;
                txtKillFLoot.Text = newEditedCommand.ItemName;
                txtKillFQty.Text = newEditedCommand.ItemQuantity;
                chkKillFTeleport.Checked = newEditedCommand.Teleport;
                checkStopItem.Checked = newEditedCommand.ItemStop;
                chkNotFoundSkip.Checked = newEditedCommand.Skip;
            }
            else if (lstCommands.SelectedItem is CommandKillRadius)
            {
                var newEditedCommand = (CommandKillRadius)lstCommands.SelectedItem;
                chkKillFTeleport.Checked = newEditedCommand.Teleport;
                numericUpDown10.Value = (decimal)newEditedCommand.Radius;
            }
            else if (lstCommands.SelectedItem is CommandBuy)
            {
                var newEditedCommand = (CommandBuy)lstCommands.SelectedItem;
                numericUpDown3.Value = newEditedCommand.ItemId;
                numericUpDown4.Value = newEditedCommand.ShopId;
                numericUpDown5.Value = newEditedCommand.ItemQty;
            }
            else if (lstCommands.SelectedItem is CommandClaimMerge)
            {
                var newEditedCommand = (CommandClaimMerge)lstCommands.SelectedItem;
                numericUpDown1.Value = newEditedCommand.MergeId;
            }
            else if (lstCommands.SelectedItem is CommandMerge)
            {
                var newEditedCommand = (CommandMerge)lstCommands.SelectedItem;
                numericUpDown2.Value = newEditedCommand.ShopId;
                numericUpDown1.Value = newEditedCommand.MergeId;
            }
            else if (lstCommands.SelectedItem is CommandSell)
            {
                var newEditedCommand = (CommandSell)lstCommands.SelectedItem;
                textBox2.Text = newEditedCommand.Item;
                numSell.Value = newEditedCommand.Quantity;
            }
            else if (lstCommands.SelectedItem is CommandUseItem)
            {
                var newEditedCommand = (CommandUseItem)lstCommands.SelectedItem;
                txtLoot.Text = newEditedCommand.Item;
            }
            else if (lstCommands.SelectedItem is CommandJoin)
            {
                var newEditedCommand = (CommandJoin)lstCommands.SelectedItem;
                numericUpDown7.Value = newEditedCommand.MapId;
                chkJoinPrivate.Checked = newEditedCommand.RequestPrivateInstance;
            }
            else if (lstCommands.SelectedItem is CommandMoveTo)
            {
                var newEditedCommand = (CommandMoveTo)lstCommands.SelectedItem;
                txtMoveNPC.Text = newEditedCommand.Npc;
                chkNextDungeonTP.Checked = newEditedCommand.Teleport;
            }
            else if (lstCommands.SelectedItem is CommandNextDungeon)
            {
                var newEditedCommand = (CommandNextDungeon)lstCommands.SelectedItem;
                chkNextDungeonTP.Checked = newEditedCommand.Teleport;
            }
            else if (lstCommands.SelectedItem is CommandWaypoint)
            {
                var newEditedCommand = (CommandWaypoint)lstCommands.SelectedItem;
                txtWaypoint.Text = newEditedCommand.Name;
                chkNextDungeonTP.Checked = newEditedCommand.Teleport;
            }
            else if (lstCommands.SelectedItem is CommandDelay)
            {
                var newEditedCommand = (CommandDelay)lstCommands.SelectedItem;
                numDelay.Value = newEditedCommand.Duration;
            }
            else if (lstCommands.SelectedItem is CommandSendPack)
            {
                var newEditedCommand = (CommandSendPack)lstCommands.SelectedItem;
                textBox1.Text = newEditedCommand.Packet;
            }
            else if (lstCommands.SelectedItem is CommandQuestAccept)
            {
                var newEditedCommand = (CommandQuestAccept)lstCommands.SelectedItem;
                numQuest.Value = newEditedCommand.QuestId;
            }
            else if (lstCommands.SelectedItem is CommandQuestComplete)
            {
                var newEditedCommand = (CommandQuestComplete)lstCommands.SelectedItem;
                numQuest.Value = newEditedCommand.QuestId;
            }
            else if (lstCommands.SelectedItem is CommandInteract)
            {
                var newEditedCommand = (CommandInteract)lstCommands.SelectedItem;
                txtMoveNPC.Text = newEditedCommand.Name;
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (lstCommands.SelectedItem is CommandKill)
            {
                var newEditedCommand = (CommandKill)lstCommands.SelectedItem;
                newEditedCommand.Monster = txtKillFMon.Text;
                newEditedCommand.Teleport = chkKillFTeleport.Checked;
                newEditedCommand.Amount = (int)KillAmount.Value;
                newEditedCommand.Text =
                    Text =
                        $"Kill {txtKillFQty.Text} x{(int)KillAmount.Value} [{(chkKillFTeleport.Checked ? "Teleport" : "Walk")}]";
                newEditedCommand.Skip = chkNotFoundSkip.Checked;
            }
            else if (lstCommands.SelectedItem is CommandKillAll)
            {
                var newEditedCommand = (CommandKillAll)lstCommands.SelectedItem;
                newEditedCommand.Teleport = chkKillFTeleport.Checked;
                newEditedCommand.Skip = chkNotFoundSkip.Checked;
            }
            else if (lstCommands.SelectedItem is CommandKillAllFor)
            {
                var newEditedCommand = (CommandKillAllFor)lstCommands.SelectedItem;
                newEditedCommand.Teleport = chkKillFTeleport.Checked;
                if (rbLoot.TabStop) newEditedCommand.IsLoot = rbLoot.TabStop;

                newEditedCommand.Monster = "*";
                newEditedCommand.ItemName = txtKillFLoot.Text;
                newEditedCommand.ItemQuantity = txtKillFQty.Text;
                newEditedCommand.Teleport = chkKillFTeleport.Checked;
                newEditedCommand.ItemStop = checkStopItem.Checked;
                newEditedCommand.Text =
                    $"Kill All for {txtKillFLoot.Text} ({txtKillFQty.Text}) [{(chkKillFTeleport.Checked ? "Teleport" : "Walk")}]";
                newEditedCommand.Skip = chkNotFoundSkip.Checked;
            }
            else if (lstCommands.SelectedItem is CommandKillFor)
            {
                var newEditedCommand = (CommandKillAllFor)lstCommands.SelectedItem;
                newEditedCommand.Teleport = chkKillFTeleport.Checked;
                if (rbLoot.TabStop) newEditedCommand.IsLoot = rbLoot.TabStop;

                newEditedCommand.Monster = txtKillFMon.Text;
                newEditedCommand.ItemName = txtKillFLoot.Text;
                newEditedCommand.ItemQuantity = txtKillFQty.Text;
                newEditedCommand.Teleport = chkKillFTeleport.Checked;
                newEditedCommand.ItemStop = checkStopItem.Checked;
                newEditedCommand.Text =
                    $"Kill {txtKillFMon.Text} for {txtKillFLoot.Text} ({txtKillFQty.Text}) [{(chkKillFTeleport.Checked ? "Teleport" : "Walk")}]";
                newEditedCommand.Skip = chkNotFoundSkip.Checked;
            }
            else if (lstCommands.SelectedItem is CommandKillRadius)
            {
                var newEditedCommand = (CommandKillRadius)lstCommands.SelectedItem;
                newEditedCommand.Teleport = chkKillFTeleport.Checked;
                newEditedCommand.Radius = (float)numericUpDown10.Value;
                newEditedCommand.Text =
                    $"Kill Radius: {(float)numericUpDown10.Value} [{(chkKillFTeleport.Checked ? "Teleport" : "Walk")}]";
            }
            else if (lstCommands.SelectedItem is CommandBuy)
            {
                var newEditedCommand = (CommandBuy)lstCommands.SelectedItem;
                newEditedCommand.ItemId = (int)numericUpDown3.Value;
                newEditedCommand.ShopId = (int)numericUpDown4.Value;
                newEditedCommand.ItemQty = (short)numericUpDown5.Value;
                newEditedCommand.Text = $"Buy {(int)numericUpDown3.Value} x{(short)numericUpDown5.Value}";
            }
            else if (lstCommands.SelectedItem is CommandClaimMerge)
            {
                var newEditedCommand = (CommandClaimMerge)lstCommands.SelectedItem;
                newEditedCommand.MergeId = (int)numericUpDown1.Value;
                newEditedCommand.Text = $"Claim Item: {decimal.ToInt32(numericUpDown1.Value)}";
            }
            else if (lstCommands.SelectedItem is CommandMerge)
            {
                var newEditedCommand = (CommandMerge)lstCommands.SelectedItem;
                newEditedCommand.ShopId = (int)numericUpDown2.Value;
                newEditedCommand.MergeId = (int)numericUpDown1.Value;
                newEditedCommand.Text = $"Craft Item: {decimal.ToInt32(numericUpDown1.Value)}";
            }
            else if (lstCommands.SelectedItem is CommandSell)
            {
                var newEditedCommand = (CommandSell)lstCommands.SelectedItem;
                newEditedCommand.Item = textBox2.Text;
                newEditedCommand.Quantity = (int)numSell.Value;
                newEditedCommand.Text = $"Sell {textBox2.Text} x{(int)numSell.Value}";
            }
            else if (lstCommands.SelectedItem is CommandUseItem)
            {
                var newEditedCommand = (CommandUseItem)lstCommands.SelectedItem;
                newEditedCommand.Item = txtLoot.Text;
                newEditedCommand.Text = $"Use: {txtLoot.Text}";
            }
            else if (lstCommands.SelectedItem is CommandJoin)
            {
                var newEditedCommand = (CommandJoin)lstCommands.SelectedItem;
                newEditedCommand.MapId = (int)numericUpDown7.Value;
                newEditedCommand.RequestPrivateInstance = chkJoinPrivate.Checked;
                newEditedCommand.Text =
                    $"Join {numericUpDown7.Value} [{(chkJoinPrivate.Checked ? "Private" : "Public")}]";
            }
            else if (lstCommands.SelectedItem is CommandMoveTo)
            {
                var newEditedCommand = (CommandMoveTo)lstCommands.SelectedItem;
                newEditedCommand.Npc = txtMoveNPC.Text;
                newEditedCommand.Teleport = chkNextDungeonTP.Checked;
                newEditedCommand.Text = $"{(chkNextDungeonTP.Checked ? "Teleport" : "Walk")} to {txtMoveNPC.Text}";
            }
            else if (lstCommands.SelectedItem is CommandNextDungeon)
            {
                var newEditedCommand = (CommandNextDungeon)lstCommands.SelectedItem;
                newEditedCommand.Teleport = chkNextDungeonTP.Checked;
                newEditedCommand.Text = $"Next dungeon [{(chkNextDungeonTP.Checked ? "Teleport" : "Walk")}]";
            }
            else if (lstCommands.SelectedItem is CommandWaypoint)
            {
                var newEditedCommand = (CommandWaypoint)lstCommands.SelectedItem;
                newEditedCommand.Name = txtWaypoint.Text;
                newEditedCommand.Teleport = chkNextDungeonTP.Checked;
                newEditedCommand.Text =
                    $"Waypoint: {txtWaypoint.Text} [{(chkNextDungeonTP.Checked ? "Teleport" : "Walk")}]";
            }
            else if (lstCommands.SelectedItem is CommandDelay)
            {
                var newEditedCommand = (CommandDelay)lstCommands.SelectedItem;
                newEditedCommand.Duration = (int)numDelay.Value;
                newEditedCommand.Text = $"Delay: {(int)numDelay.Value}";
            }
            else if (lstCommands.SelectedItem is CommandSendPack)
            {
                var newEditedCommand = (CommandSendPack)lstCommands.SelectedItem;
                newEditedCommand.Packet = textBox1.Text;
                newEditedCommand.Text = $"Send Packet: {textBox1.Text}";
            }
            else if (lstCommands.SelectedItem is CommandQuestAccept)
            {
                var newEditedCommand = (CommandQuestAccept)lstCommands.SelectedItem;
                newEditedCommand.QuestId = (int)numQuest.Value;
                newEditedCommand.Text = $"Accept quest: {(int)numQuest.Value}";
            }
            else if (lstCommands.SelectedItem is CommandQuestComplete)
            {
                var newEditedCommand = (CommandQuestComplete)lstCommands.SelectedItem;
                newEditedCommand.QuestId = (int)numQuest.Value;
                newEditedCommand.Text = $"Complete quest: {(int)numQuest.Value}";
            }
            else if (lstCommands.SelectedItem is CommandInteract)
            {
                var newEditedCommand = (CommandInteract)lstCommands.SelectedItem;
                newEditedCommand.Text = $"Interact: {txtMoveNPC.Text}";
                newEditedCommand.Npc = true;
                newEditedCommand.Name = txtMoveNPC.Text;
            }

            lstCommands.Refresh();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            lstCommands.Items.Add(new CommandInteract
            {
                Text = $"Interact: {txtMoveNPC.Text}",
                Npc = true,
                Name = txtMoveNPC.Text
            });
        }

        private void btnStatementAdd_Click(object sender, EventArgs e)
        {
            if (cbCategories.SelectedIndex > -1 && cbStatement.SelectedIndex > -1)
            {
                var value1 = txtStatement1.Text;
                lstCommands.Items.Add(new CommandStatement
                {
                    Text = $"IF {cbCategories.SelectedItem} {cbStatement.SelectedItem} {value1}",
                    Tag = cbCategories.SelectedItem.ToString(),
                    Tag2 = cbStatement.SelectedIndex,
                    Value1 = value1
                });
            }
        }

        private void cbCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCategories.SelectedItem.ToString() == "Item")
            {
                cbStatement.Items.Clear();
                cbStatement.Items.Add("Is in Inventory");
                cbStatement.Items.Add("Is not in Inventory");
                cbStatement.Items.Add("Has dropped");
                cbStatement.Items.Add("Has not dropped");
            }
            else if (cbCategories.SelectedItem.ToString() == "This Player")
            {
                cbStatement.Items.Clear();
                cbStatement.Items.Add("Gold greater than");
                cbStatement.Items.Add("Gold less than");
                cbStatement.Items.Add("Health greater than");
                cbStatement.Items.Add("Health less than");
                cbStatement.Items.Add("Level greater than");
                cbStatement.Items.Add("Level less than");
                cbStatement.Items.Add("Level is");
                cbStatement.Items.Add("Mana greater than");
                cbStatement.Items.Add("Mana less than");
                cbStatement.Items.Add("Is in Combat");
                cbStatement.Items.Add("Is not in Combat");
            }
            else if (cbCategories.SelectedItem.ToString() == "Map")
            {
                cbStatement.Items.Clear();
                cbStatement.Items.Add("player count greater than");
                cbStatement.Items.Add("player count less than");
                cbStatement.Items.Add("monster count greater than");
                cbStatement.Items.Add("monster count less than");
                cbStatement.Items.Add("contains player");
                cbStatement.Items.Add("does not contain player");
                cbStatement.Items.Add("contains monster");
                cbStatement.Items.Add("does not contain monster");
            }
            else if (cbCategories.SelectedItem.ToString() == "Quest")
            {
                cbStatement.Items.Clear();
                cbStatement.Items.Add("is complete");
                cbStatement.Items.Add("is not complete");
                cbStatement.Items.Add("is accepted");
                cbStatement.Items.Add("is not accepted");
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            lstCommands.Items.Add(new CommandEndStatement());
        }

        private void button17_Click(object sender, EventArgs e)
        {
            var machine = (KeyValuePair<string, int>)cbMachines.SelectedItem;
            lstCommands.Items.Add(new CommandMoveToMachine
            {
                Text = $"{(checkBox6.Checked ? "Teleport" : "Walk")} to {machine.Key}",
                Machine = machine.Key,
                MachineId = machine.Value,
                Teleport = checkBox6.Checked
            });
        }

        private void button16_Click(object sender, EventArgs e)
        {
            cbMachines.Items.Clear();
            if (BaseMachine.Map?.Count > 0)
                BaseMachine.Map.Values
                    .OrderBy(m =>
                        (Entities.GetInstance().me.wrapper.transform.position - m.transform.position).magnitude)
                    .ToList().ForEach(a => cbMachines.Items.Add(new KeyValuePair<string, int>(a.name, a.ID)));
            cbMachines.SelectedIndex = 0;
        }

        private void button13_Click_1(object sender, EventArgs e)
        {
            lstCommands.Items.Add(new CommandInteract
            {
                Text = $"Interact with {txtMoveNPC.Text}",
                Npc = true,
                Name = txtMoveNPC.Text
            });
        }

        private void button14_Click(object sender, EventArgs e)
        {
            var machine = (KeyValuePair<string, int>)cbMachines.SelectedItem;
            lstCommands.Items.Add(new CommandUseMachine
            {
                Text = $"Interact with {machine.Key}",
                Name = machine.Key,
                Id = machine.Value
            });
        }

        #region Untabbed

        private void cbLists_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstCommands.Visible = SelectedList == lstCommands;
            lstSpells.Visible = SelectedList == lstSpells;
            lstLoot.Visible = SelectedList == lstLoot;
            lstQuests.Visible = SelectedList == lstQuests;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            var index = SelectedList.SelectedIndex;
            if (index > -1)
                SelectedList.Items.RemoveAt(index);
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            MoveListItem(1);
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            MoveListItem(-1);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            SelectedList.Items.Clear();
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            if (ActiveBot != null)
            {
                chkStart.Checked = false;
                Bot.Instance.IsRunning = false;
                Bot.Instance.IndexChanged -= OnIndexChanged;
                Bot.Instance.IsRunningChanged -= OnIsRunningChanged;
                Object.Destroy(ActiveBot);
                ActiveBot = null;
                if (checkBox1.Checked) ChangeQuality(5, true);
                button6.Enabled = false;
            }

            lstCommands.Items.Clear();
            lstQuests.Items.Clear();
            lstLoot.Items.Clear();
            lstSpells.Items.Clear();
        }

        private void MoveListItem(int direction)
        {
            if (SelectedList.SelectedItem == null || SelectedList.SelectedIndex < 0) return;

            var newIndex = SelectedList.SelectedIndex + direction;

            if (newIndex < 0 || newIndex >= SelectedList.Items.Count) return;

            var selected = SelectedList.SelectedItem;

            SelectedList.Items.Remove(selected);
            SelectedList.Items.Insert(newIndex, selected);
            SelectedList.SetSelected(newIndex, true);
        }

        private void chkStart_Click(object sender, EventArgs e)
        {
            ToggleBot();
        }

        public void ChangeQuality(int level, bool yes)
        {
            if (Instance.checkBox1.Checked)
            {
                SettingsManager.CurrentQuality = level;
                SettingsManager.SaveGraphicsSettings(yes, yes);
            }
        }

        public void OnIndexChanged(int index)
        {
            lstCommands.SelectedIndex = index;
        }

        public void OnIsRunningChanged(bool isRunning)
        {
            chkStart.Checked = isRunning;

            if (!isRunning && ActiveBot != null)
            {
                Object.Destroy(ActiveBot);
                ActiveBot = null;
            }
        }

        #endregion

        #region Combat tab

        private void btnKillF_Click(object sender, EventArgs e)
        {
            var loot = rbLoot.Checked;
            var tp = chkKillFTeleport.Checked;
            var stop = checkStopItem.Checked;
            var name = txtKillFLoot.Text;
            var qty = txtKillFQty.Text;
            var mon = txtKillFMon.Text;

            if (name.Length > 0 && qty.Length > 0 && mon.Length > 0)
                if (qty == "*" || qty.All(char.IsDigit))
                    lstCommands.Items.Add(new CommandKillFor
                    {
                        Text = $"Kill {mon} for {name} ({qty}) [{(tp ? "Teleport" : "Walk")}]",
                        IsLoot = loot,
                        Teleport = tp,
                        ItemName = name,
                        ItemQuantity = qty,
                        ItemStop = stop,
                        Monster = mon,
                        Skip = chkNotFoundSkip.Checked
                    });
        }

        internal static void ToggleBot()
        {
            var health = Entities.GetInstance().me.statsCurrent[EntityStats.Stat.Health];
            var staffCount = 0;
            var healthCheck = false;

            if (Instance.StaffDisable.Checked)
            {
                var staff = World.Staff;
                if (staff != null) staffCount = staff.Count;
            }

            if (health <= 0.0f)
                if (ActiveBot != null)
                    healthCheck = false;

            if (!AutoLogin.IsLoggedIn || healthCheck || Instance.lstCommands.Items.Count <= 0 || staffCount > 0) return;

            if (ActiveBot == null)
            {
                if (Instance.checkBox1.Checked) Instance.ChangeQuality(1, false);
                ActiveBot = new GameObject();
                Object.DontDestroyOnLoad(ActiveBot);
                Bot.Instance = ActiveBot.AddComponent<Bot>();
                Bot.Instance.enabled = true;
                Bot.Instance.Configuration = Instance.GenerateConfig();
                Bot.Instance.IndexChanged += Instance.OnIndexChanged;
                Bot.Instance.IsRunningChanged += Instance.OnIsRunningChanged;
                Bot.Instance.IsRunning = true;
                Instance.button6.Enabled = true;
            }
            else
            {
                if (ActiveBot != null)
                {
                    Bot.Instance.IsRunning = false;
                    Bot.Instance.IndexChanged -= Instance.OnIndexChanged;
                    Bot.Instance.IsRunningChanged -= Instance.OnIsRunningChanged;
                    Object.Destroy(ActiveBot);
                    ActiveBot = null;
                    if (Instance.checkBox1.Checked) Instance.ChangeQuality(5, true);
                    Instance.button6.Enabled = false;
                }
            }
        }

        private void btnKillAll_Click(object sender, EventArgs e)
        {
            var loot = rbLoot.Checked;
            var tp = chkKillFTeleport.Checked;
            var stop = checkStopItem.Checked;
            var name = txtKillFLoot.Text;
            var qty = txtKillFQty.Text;
            var mon = txtKillFMon.Text;

            if (name.Length > 0 && qty.Length > 0 && mon.Length > 0)
                if (qty == "*" || qty.All(char.IsDigit))
                    lstCommands.Items.Add(new CommandKillAllFor
                    {
                        Text = $"Kill all for {name} ({qty}) [{(tp ? "Teleport" : "Walk")}]",
                        IsLoot = loot,
                        Teleport = tp,
                        ItemName = name,
                        ItemQuantity = qty,
                        ItemStop = stop,
                        Monster = mon,
                        Skip = chkNotFoundSkip.Checked
                    });
        }

        private void btnKill_Click(object sender, EventArgs e)
        {
            string mon;
            var tp = chkKillFTeleport.Checked;

            if ((mon = txtKillFMon.Text).Length > 0)
                lstCommands.Items.Add(new CommandKill
                {
                    Text = $"Kill {mon} x{(int)KillAmount.Value} [{(tp ? "Teleport" : "Walk")}]",
                    Monster = mon,
                    Teleport = tp,
                    Amount = (int)KillAmount.Value,
                    Skip = chkNotFoundSkip.Checked
                });
        }

        private void btnSpell_Click(object sender, EventArgs e)
        {
            if (lstSpells.Items.Count <= 10)
            {
                var spell = (int)numSpell.Value - 1;
                if (spell <= 4)
                {
                    lstSpells.Items.Add(new Spell
                    {
                        Index = spell,
                        Text = $"{spell + 1}: {Game.Instance.combat.spells.ElementAt(spell).Value.name}"
                    });
                }
                else //If Equipped Item
                {
                    spell = (int)numSpell.Value;
                    int itemID = SettingsManager.GetActionSlotIDs(spell);
                    InventoryItem inventoryItem = Session.MyPlayerData.items.FirstOrDefault((InventoryItem p) => p.ID == itemID);
                    if (itemID != 0 && inventoryItem != null)
                    {
                        lstSpells.Items.Add(new Spell
                        {
                            Index = spell,
                            Text = $"{spell}: {inventoryItem.Name}"
                        });
                    }
                }
            }
        }

        private void btnRestWait_Click(object sender, EventArgs e)
        {
            lstCommands.Items.Add(new CommandRestWait());
        }

        // private void btnRestEquip_Click(object sender, EventArgs e)
        // {
        //    string item;

        //    if ((item = txtRestItem.Text).Length > 0)
        //    {
        //        lstCommands.Items.Add(new CommandRestEquip
        //        {
        //            Text = $"Rest [Equip: {item}]",
        //            Item = item
        //        });
        //    }
        // }

        #endregion

        #region Map tab

        private void btnLoadAreas_Click(object sender, EventArgs e)
        {
            cbAreas.Items.Clear();
            cbAreas.Items.Add(new KeyValuePair<string, int>(Game.CurrentAreaName, Game.CurrentAreaID));
            if (Session.MyPlayerData.areaList != null)
            {
                Session.MyPlayerData.areaList.Where(n => n.id != Game.CurrentAreaID).OrderBy(n => n.displayName)
                    .ToList().ForEach(a => cbAreas.Items.Add(new KeyValuePair<string, int>(a.displayName, a.id)));
                cbAreas.SelectedIndex = 0;
            }
            else
            {
                btnLoadAreas.Text = "Loading...";
                btnLoadAreas.Enabled = false;
                var aec = AEC.getInstance();

                void Handler(Response r)
                {
                    if (r is ResponseAreaList)
                    {
                        ((ResponseAreaList)r).areas.Where(n => n.id != Game.CurrentAreaID).OrderBy(n => n.displayName)
                            .ToList().ForEach(
                                a => cbAreas.Items.Add(new KeyValuePair<string, int>(a.displayName, a.id)));
                        cbAreas.SelectedIndex = 0;
                        btnLoadAreas.Text = "Refresh";
                        btnLoadAreas.Enabled = true;
                        aec.ResponseReceived -= Handler;
                    }
                }

                aec.ResponseReceived += Handler;
                aec.sendRequest(new RequestAreaList());
            }

            cbAreas.SelectedIndex = 0;
        }

        private void btnJoin_Click(object sender, EventArgs e)
        {
            var index = cbAreas.SelectedIndex;

            if (index >= 0)
            {
                var area = (KeyValuePair<string, int>)cbAreas.SelectedItem;
                var priv = chkJoinPrivate.Checked;

                lstCommands.Items.Add(new CommandJoin
                {
                    Text = $"Join {area.Key} [{(priv ? "Private" : "Public")}]",
                    MapId = area.Value,
                    RequestPrivateInstance = priv
                });
            }
        }

        private void btnWaypoint_Click(object sender, EventArgs e)
        {
            string name;
            var pos = Entities.GetInstance().me.wrapper.transform.position;
            var tp = chkNextDungeonTP.Checked;

            if ((name = txtWaypoint.Text).Length > 0)
                lstCommands.Items.Add(new CommandWaypoint
                {
                    Text = $"Waypoint: {name} [{(tp ? "Teleport" : "Walk")}]",
                    Name = name,
                    Position = pos,
                    Teleport = tp
                });
        }

        private void btnMoveWalk_Click(object sender, EventArgs e)
        {
            string npc;

            if ((npc = txtMoveNPC.Text).Length > 0)
                lstCommands.Items.Add(new CommandMoveTo
                {
                    Text = $"{(chkNextDungeonTP.Checked ? "Teleport" : "Walk")} to {npc}",
                    Npc = npc,
                    Teleport = chkNextDungeonTP.Checked
                });
        }

        private void btnNextDungeon_Click(object sender, EventArgs e)
        {
            var tp = chkNextDungeonTP.Checked;

            lstCommands.Items.Add(new CommandNextDungeon
            {
                Text = $"Next dungeon [{(tp ? "Teleport" : "Walk")}]",
                Teleport = tp
            });
        }

        #endregion

        #region Quest tab

        private void btnQuestAdd_Click(object sender, EventArgs e)
        {
            var id = numQuest.Value.ToString();

            if (lstQuests.Items.Cast<string>().All(q => q != id))
                lstQuests.Items.Add(id);
        }

        private void btnQuestComplete_Click(object sender, EventArgs e)
        {
            var id = (int)numQuest.Value;

            lstCommands.Items.Add(new CommandQuestComplete
            {
                Text = $"Complete quest: {id}",
                QuestId = id
            });
        }

        private void btnQuestAccept_Click(object sender, EventArgs e)
        {
            var id = (int)numQuest.Value;

            lstCommands.Items.Add(new CommandQuestAccept
            {
                Text = $"Accept quest: {id}",
                QuestId = id
            });
        }

        #endregion

        #region Item tab

        private void btnLootAdd_Click(object sender, EventArgs e)
        {
            string loot;

            if ((loot = txtLoot.Text.Trim()).Length > 0 && lstLoot.Items.Cast<string>().All(l => l != loot))
                lstLoot.Items.Add(loot);
        }

        private void btnSell_Click(object sender, EventArgs e)
        {
            string name;

            if ((name = textBox2.Text).Length > 0)
            {
                var qty = (int)numSell.Value;

                lstCommands.Items.Add(new CommandSell
                {
                    Text = $"Sell {name} x{qty}",
                    Item = name,
                    Quantity = qty
                });
            }
        }

        #endregion

        #region Misc tab

        private void btnDelay_Click(object sender, EventArgs e)
        {
            var delay = (int)numDelay.Value;

            lstCommands.Items.Add(new CommandDelay
            {
                Text = $"Delay: {delay}",
                Duration = delay
            });
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            lstCommands.Items.Add(new CommandRestart());
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            lstCommands.Items.Add(new CommandStop());
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Load Styx bot";
                ofd.Filter = "Styx bot (*.sbot)|*.sbot";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var content = File.ReadAllText(ofd.FileName);
                    LoadBot(content);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (var ofd = new SaveFileDialog())
            {
                ofd.Title = "Save Styx bot";
                ofd.Filter = "Styx bot (*.sbot)|*.sbot";
                ofd.DefaultExt = ".sbot";
                ofd.CheckFileExists = false;

                if (ofd.ShowDialog() == DialogResult.OK)
                    SaveBot(ofd.FileName);
            }
        }

        private void LoadBot(string json)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

            var bc = JsonConvert.DeserializeObject<BotConfig>(json, settings);

            if (bc == null)
            {
                System.Windows.Forms.MessageBox.Show("Error: Unable to deserialize json (null BotConfig)", "Styx",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ApplyConfig(bc);
        }

        private void SaveBot(string file)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

            var content = JsonConvert.SerializeObject(GenerateConfig(), Formatting.Indented, settings);

            try
            {
                File.WriteAllText(file, content);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"File write operation failed: {ex.Message}", "Styx",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public BotConfig GenerateConfig()
        {
            var bc = new BotConfig
            {
                Commands = lstCommands.Items.Cast<Command>().ToList(),
                Loot = lstLoot.Items.Cast<string>().ToList(),
                Quests = lstQuests.Items.Cast<object>().Select(q => int.Parse(q.ToString())).ToList(),
                Spells = lstSpells.Items.Cast<Spell>().ToList(),
                Relogin = chkRelogin.Checked,
                Server = (string)cbServers.SelectedItem,
                BotDelay = (int)numBotDelay.Value
            };

            if (bc.Spells.All(s => s.Index != 0))
                bc.Spells.Insert(0, new Spell
                {
                    Index = 0,
                    Text = $"0: {Game.Instance.combat.spells.ElementAt(0).Value.name}"
                });

            return bc;
        }

        private void ApplyConfig(BotConfig bc)
        {
            if (bc == null) return;

            lstLoot.Items.Clear();
            lstQuests.Items.Clear();
            lstSpells.Items.Clear();
            lstCommands.Items.Clear();

            lstCommands.Items.AddRange(bc.Commands.ToArray());
            lstLoot.Items.AddRange(bc.Loot.ToArray());
            lstQuests.Items.AddRange(bc.Quests.Cast<object>().ToArray());
            lstSpells.Items.AddRange(bc.Spells.ToArray());
            chkRelogin.Checked = bc.Relogin;
            numBotDelay.Value = bc.BotDelay;

            if (cbServers.Items.Count >= 1)
                cbServers.SelectedIndex = cbServers.Items.Cast<string>().ToList().FindIndex(
                    s => s.Equals(bc.Server, StringComparison.OrdinalIgnoreCase));
        }

        #endregion
    }
}