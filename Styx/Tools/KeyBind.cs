using System;
using System.Collections.Generic;
using Styx.Core;
using Styx.UI;
using UnityEngine;

namespace Styx.Tools
{
    public class KeyBind
    {
        public static readonly List<Action> Actions = new List<Action>
        {
            () => Root.Instance.ShowForm(BotManager.Instance),
            () => Root.Instance.ShowForm(Grabbers.Instance),
            () => Root.Instance.ShowForm(Loaders.Instance),
            () => Root.Instance.ShowForm(Spammer.Instance),
            () => Root.Instance.ShowForm(Sniffer.Instance),
            () => Root.Instance.Show(KeyBinds.Instance),
            UIBankManager.Show,
            World.TeleportToCursorPosition,
            () => Unsafe.ToggleJumpHack(),
            BotManager.ToggleBot,
            () => { StyxCheat.Instance.FlyCheat.IsActive = !StyxCheat.Instance.FlyCheat.IsActive; }
        };

        public static List<KeyBind> ActiveBinds = new List<KeyBind>();

        public KeyBind(KeyCode key, int action, string text)
        {
            Key = key;
            ActionIndex = action;
            Text = text;
        }

        public KeyCode Key { get; }
        public int ActionIndex { get; }
        public string Text { get; }

        public void Register()
        {
            ActiveBinds.Add(this);
        }

        public void Unregister()
        {
            ActiveBinds.Remove(this);
        }
    }
}