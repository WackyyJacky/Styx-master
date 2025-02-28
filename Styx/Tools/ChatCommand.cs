using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Styx.Botting;
using Styx.Core;
using Styx.UI;

namespace Styx.Tools
{
    public class ChatCommand
    {
        private const string HelpMessage =
            "Available commands:\n" +
            "!help - prints available commands and their usage\n" +
            "!shop ID - loads the shop specified by ID\n" +
            "!quest ID - loads the quest specified by ID\n" +
            "!speed SPEED - sets your walk speed to SPEED (game default: 6.5)\n" +
            "!bank - opens your bank\n" +
            "!jumpheight HEIGHT - sets the jump height for you character to HEIGHT (game default: 1.5)\n" +
            "!jumphack - toggles unlimited jump/jump in the air\n" +
            "!fly - toggles fly cheat\n" +
            "!bot - toggles bot\n";

        private readonly MethodInfo _process;
        public Access Access = new Access();
        private EventInfo _chatEvent;

        public ChatCommand()
        {
            _process = GetType().GetMethod("Process", BindingFlags.Public | BindingFlags.Instance);
        }

        public void Subscribe()
        {
            try
            {
                if (AutoLogin.IsLoggedIn && AutoLogin.IsLoaded)
                {
                    if (PrivateMembers.ChatInstance.chatInput != null)
                        PrivateMembers.ChatInstance.chatInput.onChatSubmit -= PrivateMembers.ChatInstance.onChatSubmit;

                    var c = PrivateMembers.ChatInstance.chatInput;

                    _chatEvent = c.GetType().GetEvent("onChatSubmit", BindingFlags.Public | BindingFlags.Instance);

                    var fieldInfo =
                        c.GetType().GetField("onChatSubmit", BindingFlags.Instance | BindingFlags.NonPublic);

                    var field = fieldInfo.GetValue(c);

                    var eventDelegate = (MulticastDelegate)field;

                    if (eventDelegate != null)
                    {
                        var delegates = eventDelegate.GetInvocationList().ToList();

                        if (!delegates.Any(d =>
                            d.Equals(Delegate.CreateDelegate(_chatEvent.EventHandlerType, this, _process))))
                            c.onChatSubmit += Process;
                    }
                    else
                    {
                        c.onChatSubmit += Process;
                    }
                }
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show(@"Something went wrong, this is probably due to a game update. Please notify us in our Discord server.", "Styx Error: Subscribe");
            }
        }

        public void Process(string msg)
        {
            try
            {
                if (msg?.Length > 0)
                {
                    if (msg[0] != '!' && msg[0] != '/')
                    {
                        PrivateMembers.ChatInstance.onChatSubmit(msg);
                        return;
                    }

                    string[] parts;
                    string[] messageSplit;
                    if (msg[0] == '!')
                    {
                        messageSplit = msg.Split('!');
                        parts = messageSplit[1].Split(' ');
                    }
                    else
                    {
                        messageSplit = msg.Split('/');
                        parts = messageSplit[1].Split(' ');
                    }

                    switch (parts[0].ToLower())
                    {
                        case "help":
                            PrivateMembers.ChatInstance.chatText.Add(HelpMessage);
                            break;
                        case "shop":
                            if (parts.Length <= 1 || !int.TryParse(parts[1], out var id)) return;
                            UIShop.LoadShop(id);
                            break;
                        case "name":
                            Access.NewName = string.Join(" ", parts.Skip(1).Take(parts.Length - 1).ToArray());
                            Access.CheckTitles();
                            break;
                        case "title":
                            Access.NewTitle = string.Join(" ", parts.Skip(1).Take(parts.Length - 1).ToArray());
                            Access.CheckTitles();
                            break;
                        case "quest":
                            if (parts.Length <= 1 || !int.TryParse(parts[1], out var qid)) return;
                            UIQuest.ShowQuests(new List<int> { qid }, new List<int> { qid });
                            break;
                        case "speed":
                            if (parts.Length <= 1 || !float.TryParse(parts[1], out var spd)) return;
                            Entities.GetInstance().me.statsCurrent[EntityStats.Stat.RunSpeed] = spd;
                            break;
                        case "bank":
                            UIBankManager.Show();
                            break;
                        case "jumpheight":
                            if (parts.Length <= 1 || !float.TryParse(parts[1], out var height)) return;
                            Unsafe.SetJumpHeight(height, out var message);
                            PrivateMembers.ChatInstance.chatText.Add(message);
                            break;
                        case "bot":
                            BotManager.ToggleBot();
                            break;
                        case "access":
                            if (parts.Length <= 1 || !int.TryParse(parts[1], out var level)) return;
                            Access.NewAccess = level;
                            Access.CheckTitles();
                            break;
                        case "jumphack":
                            Unsafe.ToggleJumpHack(out var jMessage);
                            PrivateMembers.ChatInstance.chatText.Add(jMessage);
                            break;
                        case "customize":
                            UICharCustomize.Show();
                            break;
                        case "fly":
                            var active = StyxCheat.Instance.FlyCheat.IsActive;
                            if (active)
                            {
                                StyxCheat.Instance.FlyCheat.IsActive = false;
                                PrivateMembers.ChatInstance.chatText.Add("Fly cheat disabled");
                            }
                            else
                            {
                                StyxCheat.Instance.FlyCheat.IsActive = true;
                                PrivateMembers.ChatInstance.chatText.Add("Fly cheat enabled");
                            }
                            break;
                        case "keys":
                            PrivateMembers.ChatInstance.chatText.Add("Dungeon Key Energy: " + Session.MyPlayerData.Energy);
                            break;
                        default:
                            PrivateMembers.ChatInstance.onChatSubmit(msg);
                            break;
                    }
                }
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show(@"Something went wrong, this is probably due to a game update. Please notify us in our Discord server.", "Styx Error: Chat Commands");
            }
        }
    }
}