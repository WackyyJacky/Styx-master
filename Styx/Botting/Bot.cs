using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Styx.Botting.Commands;
using Styx.Botting.Commands.Combat;
using Styx.Botting.Commands.Item;
using Styx.Botting.Commands.Map;
using Styx.Botting.Commands.Misc;
using Styx.Botting.Commands.Quest;
using Styx.Tools;
using Styx.UI;
using UnityEngine;

namespace Styx.Botting
{
    public class Bot : MonoBehaviour, IBotEngine
    {
        public static Bot Instance;
        private readonly OmniMovementController _omc = (OmniMovementController)World.Me.moveController;
        private AutoLogin _autoLogin;
        private int _currentIndex;
        public Vector3 CurrentPos;
        private Stopwatch _delayCounter;

        private bool _hasSentRespawnRequest;

        private bool _isProcessingBotDelay;

        private bool _isRunning;
        private Stopwatch _lootDelayCounter;

        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                var changed = value != _isRunning;
                _isRunning = value;
                if (changed) IsRunningChanged?.Invoke(value);
            }
        }

        public BotConfig Configuration { get; set; }
        public event Action<int> IndexChanged;
        public event Action<bool> IsRunningChanged;

        private void StopOmc()
        {
            _omc.IsAutoRunEnabled = false;
        }

        private void Awake()
        {
            _currentIndex = 0;
            _delayCounter = new Stopwatch();
            _lootDelayCounter = new Stopwatch();
            _lootDelayCounter.Start();
            _autoLogin = new AutoLogin(Configuration.Server);
            Game.Instance.aec.ResponseReceived += ProcessAecResponse;
            Session.MyPlayerData.QuestTurnedIn += OnQuestCompleted;

            if (Configuration.Quests.Count > 0)
            {
                Game.Instance.SendQuestLoadRequest(Configuration.Quests);
                AEC.getInstance().sendRequest(new RequestQuestLoad { QuestIDs = Configuration.Quests });

                foreach (var q in Configuration.Quests)
                    if (!Session.MyPlayerData.CurQuests.Contains(q))
                        AEC.getInstance().sendRequest(new RequestQuestAccept { QuestID = q });

                foreach (var q in Configuration.Quests.Where(q => Session.MyPlayerData.IsQuestComplete(q)).ToList())
                    AEC.getInstance().sendRequest(new RequestQuestComplete { QuestID = q });
            }
        }

        private void OnDestroy()
        {
            Game.Instance.aec.ResponseReceived -= ProcessAecResponse;
            Session.MyPlayerData.QuestTurnedIn -= OnQuestCompleted;
            Instance = null;
        }

        public void OnIndexChanged(int index)
        {
            BotManager.Instance.lstCommands.SelectedIndex = index;
        }

        public void OnIsRunningChanged(bool isRunning)
        {
            BotManager.Instance.chkStart.Checked = isRunning;

            if (!isRunning && BotManager.ActiveBot != null) Destroy(BotManager.ActiveBot);
        }

        private void Update()
        {
            if (BotManager.Instance.StaffDisable.Checked)
            {
                var staff = World.Staff;
                if (staff?.Count > 0)
                    if (BotManager.Instance.chkStart.Checked || BotManager.ActiveBot != null)
                        KillBot();
            }

            if (!IsRunning) return;

            if (!AutoLogin.IsLoggedIn && Configuration.Relogin)
            {
                if (_autoLogin == null)
                    _autoLogin = new AutoLogin(Configuration.Server);
                if (!_autoLogin.Login()) return;
            }
            else if (!AutoLogin.IsLoggedIn && !Configuration.Relogin)
            {
                IsRunning = false;
                return;
            }

            if (!Respawn()) return;

            if (_isProcessingBotDelay)
                if (!ProcessBotDelay())
                    return;

            if (DialogueOverlay.mInstance != null) // True = a cutscene is active
            {
                DialogueOverlay.mInstance.Skip();
                return;
            }

            if (BotManager.Instance.checkBox7.Checked)
            {
                if (Session.MyPlayerData.CountItemsInBank(0) >= Session.MyPlayerData.BagSlots)
                {
                    MessageBox.Show("Styx", $"Your inventory is currently full. It is reommended to sell, discard, or transfer some items before continuing.");
                    ToggleBot();
                }
            }
            PickupLoot();
            CheckHp();
            CheckMp();
            if (BotManager.Instance.checkBox8.Checked)
            {
                if (Session.MyPlayerData.Energy <= 0)
                {
                    MessageBox.Show("Styx", $"You currently have zero dungeon keys.");
                    ToggleBot();
                }
            }

            if (BotManager.Instance.checkBox2.Checked) ClearEntitiesExceptMe();

            if (_currentIndex >= Configuration.Commands.Count) _currentIndex = 0;

            IndexChanged?.Invoke(_currentIndex);

            var cmd = Configuration.Commands[_currentIndex];

            if (Process(cmd))
            {
                _currentIndex++;
                if (Configuration.BotDelay > 0) _isProcessingBotDelay = true;
            }
        }

        private void ToggleBot()
        {
            if (BotManager.ActiveBot != null)
            {
                Bot.Instance.IsRunning = false;
                Bot.Instance.IndexChanged -= Instance.OnIndexChanged;
                Bot.Instance.IsRunningChanged -= Instance.OnIsRunningChanged;
                UnityEngine.Object.Destroy(BotManager.ActiveBot);
                BotManager.ActiveBot = null;
                if (BotManager.Instance.checkBox1.Checked) Instance.ChangeQuality(5, true);
                BotManager.Instance.button6.Enabled = false;
            }

        }

        private void ProcessAecResponse(Response r)
        {
            if (r is ResponseMachineUpdate)
            {
                ResponseMachineUpdate rmu;
                if ((rmu = (ResponseMachineUpdate)r).State == 1) _lastReceivedMachine = BaseMachine.Map.Values.FirstOrDefault(m => m.ID == rmu.MachineID);
            }
        }

        private bool ProcessBotDelay()
        {
            if (!_delayCounter.IsRunning)
            {
                _delayCounter.Start();
                return false;
            }

            if (_delayCounter.ElapsedMilliseconds >= Configuration.BotDelay)
            {
                _delayCounter.Reset();
                _isProcessingBotDelay = false;
                return true;
            }

            return false;
        }

        private bool Respawn()
        {
            if (World.Me.statsCurrent[EntityStats.Stat.Health] > 0 || World.Me.serverState != Entity.State.Dead)
            {
                if (!_hasSentRespawnRequest) return true;

                if (AutoLogin.IsLoaded)
                {
                    _hasSentRespawnRequest = false;
                    UIRespawnTimer timer;
                    if ((timer = FindObjectOfType<UIRespawnTimer>()) != null) timer.Visible = false;

                    return true;
                }
            }
            else
            {
                if (!_hasSentRespawnRequest && PrivateMembers.RespawnTime <= 0f)
                {
                    Game.Instance.SendRespawnRequest();
                    if (BotManager.Instance.checkDeathRestart.Checked) _currentIndex = 0;

                    _hasSentRespawnRequest = true;
                }
                else if (_hasSentRespawnRequest && PrivateMembers.RespawnTime > 0f)
                {
                    _hasSentRespawnRequest = false;
                }
            }

            return false;
        }

        private void SendChat(string chatString)
        {
            PrivateMembers.ChatInstance.chatText.Add("[BOT]: " + chatString);
        }

        private bool Process(Command cmd)
        {
            switch (cmd.Type)
            {
                case Command.CmdType.Combat:
                    return ProcessCombat(cmd);
                case Command.CmdType.Item:
                    return ProcessItem(cmd);
                case Command.CmdType.Map:
                    return ProcessMap(cmd);
                case Command.CmdType.Misc:
                    return ProcessMisc(cmd);
                case Command.CmdType.Quest:
                    return ProcessQuest(cmd);
                case Command.CmdType.Buy:
                    return ProcessBuy((CommandBuy)cmd);
                case Command.CmdType.Merge:
                    return ProcessItemMerge(cmd);
                default:
                    return false;
            }
        }

        #region Map

        private bool ProcessMap(Command command)
        {
            switch (command.Cmd)
            {
                case (int)Command.MapCommand.Join:
                    return ProcessJoin((CommandJoin)command);
                case (int)Command.MapCommand.MoveTo:
                    return ProcessMoveTo((CommandMoveTo)command);
                case (int)Command.MapCommand.NextDungeon:
                    return ProcessNextDungeon((CommandNextDungeon)command);
                case (int)Command.MapCommand.Waypoint:
                    return ProcessWaypoint((CommandWaypoint)command);
                case (int)Command.MapCommand.Interact:
                    return ProcessInteract((CommandInteract)command);
                case (int)Command.MapCommand.MoveToMachine:
                    return ProcessMoveToMachine((CommandMoveToMachine)command);
                case (int)Command.MapCommand.UseMachine:
                    return ProcessUseMachine((CommandUseMachine)command);
                default:
                    return false;
            }
        }

        private bool ProcessMoveToMachine(CommandMoveToMachine command)
        {
            var bm = (from b in BaseMachine.Map.Values
                      where b.name == command.Machine && b.ID == command.MachineId
                      select b).First();

            if (bm.name == "p_transfer_pad_Complete")
                ProcessNextDungeon(new CommandNextDungeon
                {
                    Teleport = command.Teleport
                });
            else
                ProcessWaypoint(new CommandWaypoint
                {
                    Position = bm.transform.position,
                    Teleport = command.Teleport
                });

            return true;
        }

        private bool ProcessInteract(CommandInteract command)
        {
            var npc = World.GetClosestNpc(command.Name);
            if (npc != null)
            {
                npc.Interact();
                return true;
            }

            return false;
        }

        private bool ProcessUseMachine(CommandUseMachine command)
        {
            if (BaseMachine.Map.Values.Where(n => n.name == command.Name && n.ID == command.Id).ToList().Count > 0)
            {
                AEC.getInstance().sendRequest(new RequestMachineClick(command.Id));
                return true;
            }

            return false;
        }

        private bool _hasSentJoinRequest;

        private bool ProcessJoin(CommandJoin cmd)
        {
            if (cmd.MapId == Game.Instance.AreaData.id)
                if (AutoLogin.IsLoaded)
                    if (!Loader.IsVisible)
                    {
                        _hasSentJoinRequest = false;
                        return true;
                    }

            if (!_hasSentJoinRequest)
            {
                AEC.getInstance().sendRequest(new RequestAreaJoin { areaID = cmd.MapId, IsPrivate = cmd.RequestPrivateInstance });
                _hasSentJoinRequest = true;
            }

            return false;
        }

        private bool ProcessMoveTo(CommandMoveTo cmd)
        {
            NPC npc;

            if ((npc = World.GetClosestNpc(cmd.Npc)) == null)
                return false;

            if (cmd.Teleport)
            {
                World.Me.wrapper.transform.position = npc.wrapper.transform.position;
                return true;
            }

            var dist = World.Me.combatRadius + npc.combatRadius + 2.0f;

            if ((World.Me.wrapper.transform.position - npc.wrapper.transform.position).magnitude <= dist)
            {
                StopOmc();
                return true;
            }

            if (!_omc.IsAutoRunEnabled) _omc.TargetAutoRun(npc.wrapper.transform, dist, false);

            return false;
        }

        private bool _hasEnteredMachine;
        private int _cellBeforeEnter = -1;
        private int _recordMapId = -1;
        private string _recordName = "";

        private bool ProcessNextDungeon(CommandNextDungeon cmd)
        {
            if (!_hasEnteredMachine)
                if (BaseMachine.Map?.Count > 0)
                {
                    var bm =
                        (from b in BaseMachine.Map.Values where b.name == "p_transfer_pad_Complete" select b).First() ??
                        BaseMachine.Map.Values.First();
                    _cellBeforeEnter = Game.Instance.AreaData.currentCellID;
                    _recordMapId = Game.Instance.AreaData.id;
                    _recordName = Game.Instance.AreaData.name;

                    ProcessWaypoint(new CommandWaypoint { Position = bm.transform.position, Teleport = cmd.Teleport });
                    _hasEnteredMachine = true;
                }

            if (_cellBeforeEnter != Game.Instance.AreaData.currentCellID || _recordMapId != Game.Instance.AreaData.id ||
                _recordName != Game.Instance.AreaData.name)
                if (!Loader.IsVisible && AutoLogin.IsLoaded)
                {
                    _hasEnteredMachine = false;
                    _omc.Stop();
                    return true;
                }

            return false;
        }

        private bool ProcessWaypoint(CommandWaypoint cmd)
        {
            if (cmd.Teleport)
            {
                World.Me.wrapper.transform.position = cmd.Position;
                return true;
            }

            if ((World.Me.wrapper.transform.position - cmd.Position).magnitude <= 2.0f)
            {
                StopOmc();
                return true;
            }

            var go = new GameObject();
            go.transform.position = cmd.Position;

            if (!_omc.IsAutoRunEnabled)
                _omc.TargetAutoRun(go.transform, 2.0f, false);

            return false;
        }

        #endregion

        #region Combat

        private int _spellIndex;
        public InventoryItem inventoryItem;

        private void CastSpell()
        {
            if (Configuration.Spells.Count <= 0) return;

            if (_spellIndex >= Configuration.Spells.Count)
                _spellIndex = 0;

            if ((Configuration.Spells[_spellIndex++].Index) <= 4)
            {
                Game.Instance.combat.TryCastSpell(Game.Instance.combat.spells.ElementAt(Configuration.Spells[_spellIndex].Index).Key);
            }
            else // If equipped item
            {
                //Get Item ID
                int itemID = SettingsManager.GetActionSlotIDs(Configuration.Spells[_spellIndex].Index);
                switch (itemID)
                {
                    //If HP Potion
                    case 298:
                    case 1893:
                        //Calculate Min HP Requirements to use potion
                        if (World.Me.statsCurrent[EntityStats.Stat.Health] < (int)(World.Me.statsCurrent[EntityStats.Stat.MaxHealth] - (World.Me.statsCurrent[EntityStats.Stat.MaxHealth] * 0.30)))
                        {
                            Game.Instance.SendItemUseRequest(Session.MyPlayerData.items.FirstOrDefault((InventoryItem p) => p.ID == itemID));
                        }
                        break;
                    //If MP Potion
                    case 299:
                    case 1894:
                        //Calculate Min MP Requirements to use potion
                        if (World.Me.statsCurrent[EntityStats.Stat.Resource] < (int)(World.Me.statsCurrent[EntityStats.Stat.MaxResource] - (World.Me.statsCurrent[EntityStats.Stat.MaxResource] * 0.30)))
                        {
                            Game.Instance.SendItemUseRequest(Session.MyPlayerData.items.FirstOrDefault((InventoryItem p) => p.ID == itemID));
                        }
                        break;
                    default:
                        //Use item if not a Travel Form
                        InventoryItem inventoryItem = Session.MyPlayerData.items.FirstOrDefault((InventoryItem p) => p.ID == itemID);
                        if (!inventoryItem.IsTravelForm)
                        {
                            Game.Instance.SendItemUseRequest(Session.MyPlayerData.items.FirstOrDefault((InventoryItem p) => p.ID == itemID));
                        }
                        break;
                }
            }
        }

        private bool ProcessCombat(Command command)
        {
            switch (command.Cmd)
            {
                case (int)Command.CombatCommand.Kill:
                    return ProcessKill((CommandKill)command);
                case (int)Command.CombatCommand.KillAll:
                    return ProcessKillAll((CommandKillAll)command);
                case (int)Command.CombatCommand.KillFor:
                    return ProcessKillFor((CommandKillFor)command);
                case (int)Command.CombatCommand.KillAllFor:
                    return ProcessKillAllFor((CommandKillAllFor)command);
                case (int)Command.CombatCommand.KillRadius:
                    return ProcessKillRadius((CommandKillRadius)command);
                case (int)Command.CombatCommand.RestEquip:
                    return ProcessRestEquip((CommandRestEquip)command);
                case (int)Command.CombatCommand.RestWait:
                    return ProcessRestWait((CommandRestWait)command);
                case (int)Command.CombatCommand.RestTeleport:
                    return ProcessRestTeleport((CommandRestTeleport)command);
                default:
                    return false;
            }
        }

        private bool ProcessKillRadius(CommandKillRadius command)
        {
            var mon = World.GetClosestMonsterNew("*", command.Radius, command.Position);
            if (World.Me.Target != null)
            {
                ProcessKill(new CommandKill
                {
                    Monster = World.Me.Target.name,
                    Teleport = command.Teleport,
                    Radius = command.Radius,
                    PlayerPos = command.Position
                });
                return false;
            }

            if (mon != null)
            {
                ProcessKill(new CommandKill
                {
                    Monster = mon.name,
                    Teleport = command.Teleport,
                    Radius = command.Radius,
                    PlayerPos = command.Position
                });
                return false;
            }

            return true;
        }

        private bool ProcessRestTeleport(CommandRestTeleport command)
        {
            if (World.Me.wrapper.transform.position == command.Position)
            {
                var cur = World.Me.statsCurrent[EntityStats.Stat.Health];
                var max = World.Me.statsCurrent[EntityStats.Stat.MaxHealth];
                var curM = World.Me.statsCurrent[EntityStats.Stat.Resource];
                var maxM = World.Me.statsCurrent[EntityStats.Stat.MaxResource];
                if (Math.Abs(World.Me.statsCurrent[EntityStats.Stat.MaxResource]) <= 0)
                {
                    if (cur >= max)
                    {
                        World.Me.wrapper.transform.position = CurrentPos;
                        return true;
                    }
                }
                else
                {
                    if (cur >= max && curM >= maxM)
                    {
                        World.Me.wrapper.transform.position = CurrentPos;
                        return true;
                    }
                }
            }
            else
            {
                if (CurrentPos != World.Me.wrapper.transform.position) CurrentPos = World.Me.wrapper.transform.position;

                World.Me.wrapper.transform.position = command.Position;
            }

            return false;
        }

        public enum Combat
        {
            Idle,
            Attacking,
            Killed
        }

        private Combat _combatStatus = Combat.Idle;

        private void OnTargetKilled(Entity e)
        {
            _combatStatus = Combat.Killed;
            e.DeathEvent -= OnTargetKilled;
        }

        private int _killAmount = 0;

        private bool ProcessKill(CommandKill cmd)
        {
            if (cmd.Amount <= 0) cmd.Amount = 1;
            if (_combatStatus != Combat.Killed)
            {
                var target = World.Me.Target;
                if (target == null || cmd.Monster != "*" &&
                    !target.name.Equals(cmd.Monster, StringComparison.OrdinalIgnoreCase))
                {
                    var mon = World.GetClosestMonsterNew(cmd.Monster, cmd.Radius, cmd.PlayerPos);
                    if (mon != null)
                    {
                        if (cmd.Teleport)
                        {
                            var dist = (World.Me.wrapper.transform.position - mon.wrapper.transform.position).magnitude;
                            if (dist > World.Me.combatRadius)
                                World.Me.wrapper.transform.position = mon.wrapper.transform.position;
                        }
                        else if (BotManager.Instance.checkBox3.Checked &&
                                 (World.Me.wrapper.transform.position - mon.wrapper.transform.position).magnitude <=
                                 World.Me.combatRadius + mon.combatRadius + 2.0f)
                        {
                            ProcessMoveTo(new CommandMoveTo
                            {
                                Npc = mon.name,
                                Teleport = cmd.Teleport
                            });
                        }

                        World.Me.Target = mon;
                        mon.DeathEvent += OnTargetKilled;
                        _combatStatus = Combat.Attacking;
                    }
                    else
                    {
                        if (cmd.Skip) return true;
                    }

                    return false;
                }

                if (World.PlayerHasValidTarget((NPC)World.Me.Target))
                {
                    if (cmd.Radius != 0 && cmd.PlayerPos != null)
                    {
                        var m = (NPC)World.Me.Target;
                        var closest = World.GetClosestMonsterNew(cmd.Monster, cmd.Radius, cmd.PlayerPos);
                        if ((cmd.PlayerPos - m.wrapper.transform.position).magnitude > cmd.Radius)
                        {
                            StopOmc();
                            World.Me.Target = null;
                            return false;
                        }
                    }

                    CastSpell();
                    return false;
                }
            }

            _combatStatus = Combat.Idle;
            if (_killAmount == cmd.Amount)
            {
                _killAmount = 0;
                return true;
            }
            else
            {
                _killAmount++;
                return false;
            }
        }

        private bool ProcessKillAll(CommandKillAll cmd)
        {
            var monsters = World.Monsters;

            if (monsters.Count <= 0) return true;

            ProcessKill(new CommandKill
            {
                Monster = "*",
                Teleport = cmd.Teleport,
                Skip = cmd.Skip
            });

            return false;
        }

        private bool ProcessKillFor(CommandKillFor cmd)
        {
            if (cmd.IsLoot)
            {
                InventoryItem i;
                if ((i = World.GetInventoryItem(cmd.ItemName)) != null)
                    if (cmd.ItemQuantity == "*" || i.Qty >= int.Parse(cmd.ItemQuantity))
                    {
                        if (cmd.ItemStop) KillBot();
                        return true;
                    }
            }
            else
            {
                QuestObjective q;
                if ((q = World.GetQuestObjective(cmd.ItemName)) != null)
                    if (cmd.ItemQuantity == "*" ||
                        Session.MyPlayerData.GetQuestObjectiveProgress(q) >= int.Parse(cmd.ItemQuantity))
                    {
                        if (cmd.ItemStop) KillBot();
                        return true;
                    }
            }

            ProcessKill(new CommandKill
            {
                Monster = cmd.Monster,
                Teleport = cmd.Teleport,
                Skip = cmd.Skip
            });

            return false;
        }

        private bool ProcessKillAllFor(CommandKillAllFor cmd)
        {
            var monsters = World.Monsters;

            if (monsters.Count <= 0) return true;

            if (cmd.IsLoot)
            {
                InventoryItem i;
                if ((i = World.GetInventoryItem(cmd.ItemName)) != null)
                    if (cmd.ItemQuantity == "*" || i.Qty >= int.Parse(cmd.ItemQuantity))
                        if (cmd.ItemStop)
                            KillBot();
                        else
                            return true;
            }
            else
            {
                QuestObjective q;
                if ((q = World.GetQuestObjective(cmd.ItemName)) != null)
                    if (cmd.ItemQuantity == "*" ||
                        Session.MyPlayerData.GetQuestObjectiveProgress(q) >= int.Parse(cmd.ItemQuantity))
                        if (cmd.ItemStop)
                            KillBot();
                        else
                            return true;
            }

            ProcessKill(new CommandKill
            {
                Monster = "*",
                Teleport = cmd.Teleport,
                Skip = cmd.Skip
            });
            return false;
        }

        public void KillBot()
        {
            Instance.IsRunning = false;
            Instance.IndexChanged -= OnIndexChanged;
            Instance.IsRunningChanged -= OnIsRunningChanged;
            Destroy(BotManager.ActiveBot);
            BotManager.ActiveBot = null;
            if (BotManager.Instance.checkBox1.Checked) ChangeQuality(5, true);

            BotManager.Instance.chkStart.Checked = false;
        }

        public void ChangeQuality(int level, bool yes)
        {
            try
            {
                if (BotManager.Instance.checkBox1.Checked)
                    if (level != SettingsManager.CurrentQuality)
                    {
                        SettingsManager.CurrentQuality = level;
                        SettingsManager.SaveGraphicsSettings(yes, yes);
                    }
            }
            catch (Exception ex)
            {
                SendChat(ex.Message);
            }
        }

        private bool _equipActionTaken;

        private bool ProcessRestEquip(CommandRestEquip cmd)
        {
            InventoryItem i;

            if ((i = World.GetInventoryItem(cmd.Item)) == null)
                return true;

            if (_equipActionTaken)
            {
                if (PrivateMembers.IsAssetTransforming)
                    return false;
                _equipActionTaken = false;
                return true;
            }

            if (i.IsEquipped)
            {
                Game.Instance.SendUnequipRequest(i.CharItemID);
                _equipActionTaken = true;
            }
            else
            {
                Game.Instance.SendEquipRequest(i.CharItemID, i.EquipID);
                _equipActionTaken = true;
            }

            return false;
        }

        private bool ProcessRestWait(CommandRestWait cmd)
        {
            var cur = World.Me.statsCurrent[EntityStats.Stat.Health];
            var max = World.Me.statsCurrent[EntityStats.Stat.MaxHealth];
            var curM = World.Me.statsCurrent[EntityStats.Stat.Resource];
            var maxM = World.Me.statsCurrent[EntityStats.Stat.MaxResource];
            var dist = World.Me.combatRadius + 5.0f;

            var mon = World.GetClosestMonsterNew("*", dist, World.Me.wrapper.transform.position);

            if (mon != null && mon.IsAlive() && mon.Target.isMe && World.Me.CanAttack(mon))
            {
                if ((World.Me.wrapper.transform.position - mon.wrapper.transform.position).magnitude <=
                    World.Me.combatRadius + mon.combatRadius + 2.0f)
                    ProcessMoveTo(new CommandMoveTo
                    {
                        Npc = mon.name,
                        Teleport = false
                    });

                ProcessKill(new CommandKill
                {
                    Monster = "*",
                    Teleport = false,
                    Skip = false
                });
            }

            if (Math.Abs(World.Me.statsCurrent[EntityStats.Stat.MaxResource]) <= 0)
            {
                return cur >= max;
            }

            return cur >= max && curM >= maxM;
        }

        #endregion

        #region Item

        private void PickupLoot()
        {
            if (_lootDelayCounter.ElapsedMilliseconds >= 500)
            {
                var loot = PrivateMembers.LootBags;
                if (loot?.Count > 0)
                    foreach (var cl in loot)
                        cl.Items.ForEach(lootItem =>
                        {
                            if (Configuration.Loot.Any(l =>
                                    l.Equals(lootItem.Name.Trim(), StringComparison.OrdinalIgnoreCase)) ||
                                Configuration.Loot.Any(l => l.Equals("*")))
                            {
                                Game.Instance.SendLootItemRequest(cl.ID, lootItem.ID, lootItem.Qty);
                                if (BotManager.Instance.checkLootChat.Checked)
                                    SendChat("Found " + lootItem.Qty + " " + lootItem.Name.Trim());
                            }
                        });

                _lootDelayCounter.Reset();
                _lootDelayCounter.Start();
            }
        }

        private bool ProcessItem(Command command)
        {
            switch (command.Cmd)
            {
                case (int)Command.ItemCommand.Sell:
                    return ProcessSell((CommandSell)command);
                case (int)Command.ItemCommand.Use:
                    return ProcessUseItem((CommandUseItem)command);
                default:
                    return false;
            }
        }

        private void CheckHp()
        {
            if (BotManager.Instance.chkPotionUse.Checked)
            {
                if (World.Me.statsCurrent[EntityStats.Stat.Health] < (float)BotManager.Instance.numHPBelow.Value && World.Me.statsCurrent[EntityStats.Stat.Health] < World.Me.statsCurrent[EntityStats.Stat.MaxHealth])
                {
                    InventoryItem i;
                    if (BotManager.Instance.checkBox4.Checked)
                    {
                        if ((i = World.GetInventoryItem("Health Potion")) != null)
                        {
                            Game.Instance.SendItemUseRequest(i);
                            _boughtPotions = false;
                        }
                        else
                        {
                            if (BotManager.Instance.checkBuyPotions.Checked)
                            {
                                if (!_boughtPotions)
                                {
                                    ProcessBuy(new CommandBuy
                                    {
                                        ShopId = 12,
                                        ItemId = 1893,
                                        ItemQty = (short)BotManager.Instance.numericUpDown6.Value
                                    });
                                    _boughtPotions = true;
                                    CheckHp();
                                }
                            }
                        }
                    }
                    else if (BotManager.Instance.checkBox5.Checked)
                    {
                        if ((i = World.GetInventoryItem("Small Health Potion")) != null)
                        {
                            Game.Instance.SendItemUseRequest(i);
                            _boughtPotions = false;
                        }
                        else
                        {
                            if (BotManager.Instance.checkBuyPotions.Checked)
                            {
                                if (!_boughtPotions)
                                {
                                    ProcessBuy(new CommandBuy
                                    {
                                        ShopId = 12,
                                        ItemId = 298,
                                        ItemQty = (short)BotManager.Instance.numericUpDown6.Value
                                    });
                                    _boughtPotions = true;
                                    CheckHp();
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CheckMp()
        {
            if (BotManager.Instance.chkMPPotionUse.Checked && Math.Abs(World.Me.statsCurrent[EntityStats.Stat.MaxResource]) > 0)
            {
                if (World.Me.statsCurrent[EntityStats.Stat.Resource] < (float)BotManager.Instance.numMPBelow.Value && World.Me.statsCurrent[EntityStats.Stat.Resource] < World.Me.statsCurrent[EntityStats.Stat.MaxResource])
                {
                    InventoryItem i;
                    if (BotManager.Instance.checkBox4.Checked)
                    {
                        if ((i = World.GetInventoryItem("Mana Potion")) != null)
                        {
                            Game.Instance.SendItemUseRequest(i);
                            _boughtPotions = false;
                        }
                        else
                        {
                            if (BotManager.Instance.checkBuyPotions.Checked)
                            {
                                if (!_boughtPotions)
                                {
                                    ProcessBuy(new CommandBuy
                                    {
                                        ShopId = 12,
                                        ItemId = 1894,
                                        ItemQty = (short)BotManager.Instance.numericUpDown8.Value
                                    });
                                    _boughtPotions = true;
                                    CheckMp();
                                }
                            }
                        }
                    }
                    else if (BotManager.Instance.checkBox5.Checked)
                    {
                        if ((i = World.GetInventoryItem("Small Mana Potion")) != null)
                        {
                            Game.Instance.SendItemUseRequest(i);
                            _boughtPotions = false;
                        }
                        else
                        {

                            if (BotManager.Instance.checkBuyPotions.Checked)
                            {
                                if (!_boughtPotions)
                                {
                                    ProcessBuy(new CommandBuy
                                    {
                                        ShopId = 12,
                                        ItemId = 299,
                                        ItemQty = (short)BotManager.Instance.numericUpDown8.Value
                                    });
                                    _boughtPotions = true;
                                    CheckMp();
                                }
                            }
                        }
                    }
                }
            }
        }
        private bool ProcessItemMerge(Command command)
        {
            switch (command.Cmd)
            {
                case (int)Command.MergeCommand.Merge:
                    return ProcessMerge((CommandMerge)command);
                case (int)Command.MergeCommand.ClaimMerge:
                    return ProcessClaimMerge((CommandClaimMerge)command);
                default:
                    return false;
            }
        }

        private int _qtYoriginal = -1;

        private bool ProcessBuy(CommandBuy command)
        {
            InventoryItem inventoryItem = (from p in Session.MyPlayerData.items where p.ID == command.ItemId select p)
                .FirstOrDefault<InventoryItem>();
            if (_qtYoriginal == -1)
            {
                if (inventoryItem == null || inventoryItem.MaxStack > 1 &&
                    Session.MyPlayerData.CountItemsInBank(0) < Session.MyPlayerData.BagSlots &&
                    inventoryItem.MaxStack <= 1 && inventoryItem.Qty < inventoryItem.MaxStack &&
                    inventoryItem.Cost <= Session.MyPlayerData.Gold)
                {
                    _qtYoriginal = inventoryItem?.Qty ?? 0;
                    Game.Instance.SendBuyRequest(command.ShopId, command.ItemId, command.ItemQty);
                }
            }
            else
            {
                if (inventoryItem != null && inventoryItem.Qty > _qtYoriginal)
                {
                    _qtYoriginal = -1;
                    return true;
                }
            }
            return false;
        }

        private bool ProcessClaimMerge(CommandClaimMerge command)
        {
            var inventoryItem = (from p in Session.MyPlayerData.items where p.ID == command.MergeId select p)
                .FirstOrDefault();
            if ((inventoryItem == null || inventoryItem.MaxStack <= 1) &&
                Session.MyPlayerData.CountItemsInBank(0) >= Session.MyPlayerData.BagSlots)
            {
                Notification.ShowText("Inventory is full!");
            }
            else if (inventoryItem != null && inventoryItem.MaxStack > 1 && inventoryItem.Qty >= inventoryItem.MaxStack)
            {
                Notification.ShowText("Stack is full!");
            }
            else
            {
                Game.Instance.SendMergeClaimRequest(command.MergeId);
                SendChat("Merge Claim Request Sent");
            }

            return true;
        }

        private bool ProcessMerge(CommandMerge command)
        {
            var inventoryItem = (from p in Session.MyPlayerData.items where p.ID == command.MergeId select p)
                .FirstOrDefault();
            if ((inventoryItem == null || inventoryItem.MaxStack <= 1) &&
                Session.MyPlayerData.CountItemsInBank(0) >= Session.MyPlayerData.BagSlots)
            {
                SendChat("Inventory is full!");
            }
            else if (inventoryItem != null && inventoryItem.MaxStack > 1 && inventoryItem.Qty >= inventoryItem.MaxStack)
            {
                SendChat("Stack is full!");
            }
            else
            {
                UIMerge.Load(command.ShopId);
                if (UIMerge.currentShopID != command.ShopId) return false;

                var m = (from p in UIMerge.currentShop.Merges.Values
                         where p.IsVisibleInStore() && p.MergeID == command.MergeId
                         orderby p.SortOrder
                         select p).FirstOrDefault();
                if (m.MergeID == command.MergeId)
                {
                    var text = Session.MyPlayerData.CanCraft(m);
                    if (!m.IsMC)
                    {
                        SendChat($"{m.Name} is not eligible for crafting");
                    }
                    else if (m.IsGuardian && !Session.MyPlayerData.IsGuardian())
                    {
                        SendChat("Guardian Only Item");
                    }
                    else if (!string.IsNullOrEmpty(text))
                    {
                        SendChat(text);
                    }
                    else
                    {
                        AEC.getInstance().sendRequest(new RequestMerge
                        {
                            ShopID = command.ShopId,
                            MergeID = command.MergeId
                        });
                        SendChat($"Crafting {m.Name}");
                    }
                }

                UIWindow.ClearWindows();
            }

            return true;
        }

        private InventoryItem _itemToSell;
        private InventoryItem _itemToSellOriginal;
        private bool _hasSoldItem;

        private bool ProcessSell(CommandSell cmd)
        {
            if (_itemToSellOriginal == null)
            {
                _itemToSellOriginal = World.GetInventoryItem(cmd.Item);
                if (_itemToSellOriginal == null)
                {
                    ResetSell();
                    return true;
                }
            }
            else
            {
                _itemToSell = World.GetInventoryItem(cmd.Item);
                if (_itemToSell == null || _itemToSell.Qty != _itemToSellOriginal.Qty || _itemToSell.IsEquipped)
                {
                    ResetSell();
                    return true;
                }
            }

            if (!_hasSoldItem)
            {
                Game.Instance.SendSellRequest(_itemToSell.CharItemID,
                    _itemToSell.Qty < cmd.Quantity ? _itemToSell.Qty : (short)cmd.Quantity);
                _hasSoldItem = true;
            }

            ResetSell();
            return true;
        }

        private bool ProcessUseItem(CommandUseItem command)
        {
            InventoryItem i;
            if ((i = World.GetInventoryItem(command.Item)) != null)
            {
                Game.Instance.SendItemUseRequest(i);
                return true;
            }

            return false;
        }

        private void ResetSell()
        {
            _hasSoldItem = false;
            _itemToSell = null;
            _itemToSellOriginal = null;
        }

        #endregion

        #region Misc

        private bool ProcessMisc(Command command)
        {
            switch (command.Cmd)
            {
                case (int)Command.MiscCommand.Delay:
                    return ProcessDelay((CommandDelay)command);
                case (int)Command.MiscCommand.Restart:
                    return ProcessRestart((CommandRestart)command);
                case (int)Command.MiscCommand.Stop:
                    return ProcessStop((CommandStop)command);
                case (int)Command.MiscCommand.SendPacket:
                    return ProcessSendPacket((CommandSendPack)command);
                case (int)Command.MiscCommand.IfStatement:
                    return ProcessIfStatement((CommandStatement)command);
                case (int)Command.MiscCommand.EndIfStatement:
                    return ProcessEndStatement((CommandEndStatement)command);
                default:
                    return false;
            }
        }

        private bool ProcessSendPacket(CommandSendPack command)
        {
            MethodInfo sendMessage;
            var requestParams = new List<object[]>();
            var req = JsonConvert.DeserializeObject<Request>(command.Packet);
            requestParams.Add(new object[] { Encoding.ASCII.GetBytes(command.Packet), req.type, req.cmd });
            sendMessage = Game.Instance.aec.GetType()
                .GetMethod("sendMessage", BindingFlags.NonPublic | BindingFlags.Instance);
            sendMessage.Invoke(Game.Instance.aec, requestParams[0]);
            return true;
        }

        private bool ProcessEndStatement(CommandEndStatement command)
        {
            return true;
        }

        private bool ProcessIfStatement(CommandStatement command)
        {
            switch (command.Tag)
            {
                case "Item":
                    return ProcessIfItem(command);
                case "This Player":
                    return ProcessIfThisPlayer(command);
                case "Map":
                    return ProcessIfMap(command);
                case "Quest":
                    return ProcessIfQuest(command);
                default:
                    return false;
            }
        }

        private bool EndIfStatement(string searchString, int initialIndex)
        {
            var x = -1;
            if (searchString.Length != 0)
            {
                x = BotManager.Instance.lstCommands.FindString(searchString, initialIndex);
                if (x != -1)
                {
                    BotManager.Instance.lstCommands.SetSelected(x, true);
                    _currentIndex = x;
                    return true;
                }
            }

            return true;
        }

        private bool ProcessIfMap(CommandStatement command)
        {
            int.TryParse(command.Value1, out var value);
            var found = false;
            switch (command.Tag2)
            {
                case 0: // player count greater than
                    if (Entities.GetInstance().playerMap.Count > value) return true;
                    return EndIfStatement("EndIF", _currentIndex);
                case 1: // player count less than
                    if (Entities.GetInstance().playerMap.Count < value) return true;
                    return EndIfStatement("EndIF", _currentIndex);
                case 2: // monster count greater than
                    var monsters = World.Monsters
                        .Where(m => World.Me.CanAttack(m) && m.IsAlive() && m.HealthPercent > 0f).ToList();
                    if (monsters != null && monsters.Count() > value) return true;
                    return EndIfStatement("EndIF", _currentIndex);
                case 3: // monster count less than
                    var monsters2 = World.Monsters
                        .Where(m => World.Me.CanAttack(m) && m.IsAlive() && m.HealthPercent > 0f).ToList();
                    if (monsters2 == null) return true;
                    else if (monsters2 != null && monsters2.Count() < value) return true;
                    return EndIfStatement("EndIF", _currentIndex);
                case 4: // contains player
                    found = false;
                    if (Entities.GetInstance().playerMap.Count > 0)
                    {
                        foreach (var keyValuePair2 in Entities.GetInstance().playerMap)
                            if (keyValuePair2.Value.name == command.Value1)
                                found = true;
                        if (found) return true;
                    }

                    return EndIfStatement("EndIF", _currentIndex);
                case 5: // does not contain player
                    found = false;
                    if (Entities.GetInstance().playerMap.Count > 0)
                    {
                        foreach (var keyValuePair2 in Entities.GetInstance().playerMap)
                            if (keyValuePair2.Value.name == command.Value1)
                                found = true;
                        if (!found) return true;
                    }
                    else
                    {
                        return true;
                    }

                    return EndIfStatement("EndIF", _currentIndex);
                case 6: // contains monster
                    if (World.GetClosestMonster(command.Value1) != null) return true;
                    return EndIfStatement("EndIF", _currentIndex);
                case 7: // does not contains monster
                    if (World.GetClosestMonster(command.Value1) == null) return true;
                    return EndIfStatement("EndIF", _currentIndex);
                default:
                    return EndIfStatement("EndIF", _currentIndex);
            }
        }

        private bool ProcessIfItem(CommandStatement command)
        {
            var loot = PrivateMembers.LootBags;
            var found = false;
            InventoryItem inventoryItem;
            switch (command.Tag2)
            {
                case 0: // Is in Inventory
                    inventoryItem = (from p in Session.MyPlayerData.items where p.Name == command.Value1 select p).FirstOrDefault();
                    if (inventoryItem != null) return true;
                    return EndIfStatement("EndIF", _currentIndex);
                case 1: // Is not in Inventory
                    inventoryItem = (from p in Session.MyPlayerData.items where p.Name == command.Value1 select p).FirstOrDefault();
                    if (inventoryItem != null) return true;
                    return EndIfStatement("EndIF", _currentIndex);
                case 2: // Has dropped
                    if (loot?.Count > 0)
                    {
                        foreach (var cl in loot)
                            cl.Items.ForEach(lootItem =>
                            {
                                if (lootItem.Name.Equals(command.Value1.Trim()) || command.Value1 == "*") found = true;
                            });

                        if (found) return true;
                    }
                    return EndIfStatement("EndIF", _currentIndex);
                case 3: // Has not dropped
                    if (loot?.Count > 0)
                    {
                        foreach (var cl in loot)
                            cl.Items.ForEach(lootItem =>
                            {
                                if (lootItem.Name.Equals(command.Value1.Trim()) || command.Value1 == "*") found = true;
                            });

                        if (!found) return true;
                    }
                    else
                    {
                        return true;
                    }
                    return EndIfStatement("EndIF", _currentIndex);
                default:
                    return EndIfStatement("EndIF", _currentIndex);
            }
        }

        private bool ProcessIfThisPlayer(CommandStatement command)
        {
            int.TryParse(command.Value1, out var value);
            switch (command.Tag2)
            {
                case 0: // Gold greater than
                    if (Session.MyPlayerData.Gold > value) return true;
                    return EndIfStatement("EndIF", _currentIndex);
                case 1: // Gold less than
                    if (Session.MyPlayerData.Gold < value) return true;
                    return EndIfStatement("EndIF", _currentIndex);

                case 2: // Health Greater Than
                    if (World.Me.statsCurrent[EntityStats.Stat.Health] > value) return true;
                    return EndIfStatement("EndIF", _currentIndex);

                case 3: // Health less Than
                    if (World.Me.statsCurrent[EntityStats.Stat.Health] < value) return true;
                    return EndIfStatement("EndIF", _currentIndex);

                case 4: // Level greater than
                    if (World.Me.Level > value) return true;
                    return EndIfStatement("EndIF", _currentIndex);

                case 5: // Level less than
                    if (World.Me.Level < value) return true;
                    return EndIfStatement("EndIF", _currentIndex);

                case 6: // Level is
                    if (World.Me.Level == value) return true;
                    return EndIfStatement("EndIF", _currentIndex);

                case 7: // Mana greater than
                    if (World.Me.statsCurrent[EntityStats.Stat.Resource] > value) return true;
                    return EndIfStatement("EndIF", _currentIndex);

                case 8: // Mana less than
                    if (World.Me.statsCurrent[EntityStats.Stat.Resource] < value) return true;
                    return EndIfStatement("EndIF", _currentIndex);
                case 9: // Is in Combat
                    if (World.Me.serverState == Entity.State.InCombat) return true;
                    return EndIfStatement("EndIF", _currentIndex);

                case 10: // Is not in Combat
                    if (World.Me.serverState == Entity.State.InCombat) return true;
                    return EndIfStatement("EndIF", _currentIndex);
                default:
                    return EndIfStatement("EndIF", _currentIndex);
            }
        }

        private bool ProcessIfQuest(CommandStatement command)
        {
            if (int.TryParse(command.Value1, out var value))
                switch (command.Tag2)
                {
                    case 0: // Quest Complete
                        if (Session.MyPlayerData.IsQuestComplete(value)) return true;
                        return EndIfStatement("EndIF", _currentIndex);
                    case 1: // Quest not complete
                        if (!Session.MyPlayerData.IsQuestComplete(value)) return true;
                        return EndIfStatement("EndIF", _currentIndex);
                    case 2: // Quest in progress
                        if (Session.MyPlayerData.CurQuests.Contains(value)) return true;
                        return EndIfStatement("EndIF", _currentIndex);
                    case 3: // Quest not in progress
                        if (!Session.MyPlayerData.CurQuests.Contains(value)) return true;
                        return EndIfStatement("EndIF", _currentIndex);
                    default:
                        return EndIfStatement("EndIF", _currentIndex);
                }
            return false;
        }

        private bool ProcessDelay(CommandDelay cmd)
        {
            if (!_delayCounter.IsRunning)
            {
                _delayCounter.Start();
                return false;
            }

            if (_delayCounter.ElapsedMilliseconds >= cmd.Duration)
            {
                _delayCounter.Reset();
                return true;
            }

            return false;
        }

        public void ClearEntitiesExceptMe()
        {
            if (Entities.GetInstance().playerMap.Count > 0)
            {
                foreach (var keyValuePair2 in Entities.GetInstance().playerMap)
                    if (keyValuePair2.Value != World.Me)
                        keyValuePair2.Value.Dispose();

                Entities.GetInstance().playerMap = new Dictionary<int, Player>();
                if (World.Me != null) Entities.GetInstance().addPlayer(World.Me);
            }
        }

        private bool ProcessRestart(CommandRestart cmd)
        {
            _currentIndex = -1;
            return true;
        }

        private bool ProcessStop(CommandStop cmd)
        {
            IsRunning = false;
            return true;
        }

        #endregion

        #region Quest

        private void OnQuestCompleted(int id)
        {
            Game.Instance.SendQuestAcceptRequest(id);
        }

        private bool ProcessQuest(Command command)
        {
            switch (command.Cmd)
            {
                case (int)Command.QuestCommand.Accept:
                    return ProcessAccept((CommandQuestAccept)command);
                case (int)Command.QuestCommand.Complete:
                    return ProcessComplete((CommandQuestComplete)command);
                default:
                    return false;
            }
        }

        private bool _questAcceptSent;
        private bool _questAccepted;

        private bool ProcessAccept(CommandQuestAccept cmd)
        {
            if (_questAccepted || Session.MyPlayerData.CurQuests.Contains(cmd.QuestId))
            {
                _questAccepted = false;
                _questAcceptSent = false;
                return true;
            }

            if (!_questAcceptSent)
            {
                Game.Instance.SendQuestAcceptRequest(cmd.QuestId);
                Session.MyPlayerData.QuestAdded += OnQuestAccepted;
                _questAcceptSent = true;
            }

            return false;
        }

        private void OnQuestAccepted(int id)
        {
            _questAccepted = true;
            Session.MyPlayerData.QuestAdded -= OnQuestAccepted;
        }

        private bool _questCompleteSent;
        private bool _questCompleted;
        private BaseMachine _lastReceivedMachine;
        private bool _boughtPotions;

        private bool ProcessComplete(CommandQuestComplete cmd)
        {
            if (!_questCompleteSent)
            {
                Game.Instance.SendQuestCompleteRequest(cmd.QuestId);
                Session.MyPlayerData.QuestRemoved += OnQuestRemoved;
                _questCompleteSent = true;
            }

            if (_questCompleted)
            {
                _questCompleted = false;
                _questCompleteSent = false;
                return true;
            }

            return false;
        }

        private void OnQuestRemoved(int id)
        {
            _questCompleted = true;
            Session.MyPlayerData.QuestRemoved -= OnQuestAccepted;
        }

        #endregion
    }
}