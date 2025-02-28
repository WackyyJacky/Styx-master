using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Styx.Tools.Hooking;
using UnityEngine;

namespace Styx.Tools
{
    public class FlyCheat
    {
        private readonly Dictionary<InputAction, KeyMap> _userKeys;
        private bool _isActive;

        public FlyCheat()
        {
            _userKeys = Clone(SettingsManager.KeyMappings);
        }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive = value)
                {
                    DisableUserKeys();
                    InstallHook();
                }
                else
                {
                    EnableUserKeys();
                    UninstallHook();
                }
            }
        }

        public KeyCode ForwardKey => _userKeys[InputAction.Forward].MappedKey;
        public KeyCode BackwardKey => _userKeys[InputAction.Backward].MappedKey;
        public KeyCode JumpKey => _userKeys[InputAction.Jump].MappedKey;
        public KeyCode LeftStrafeKey => _userKeys[InputAction.LeftStrafe].MappedKey;
        public KeyCode RightStrafeKey => _userKeys[InputAction.RightStrafe].MappedKey;

        private void DisableUserKeys()
        {
            SettingsManager.KeyMappings[InputAction.Forward].MappedKey = KeyCode.None;
            SettingsManager.KeyMappings[InputAction.Backward].MappedKey = KeyCode.None;
            SettingsManager.KeyMappings[InputAction.Jump].MappedKey = KeyCode.None;
            SettingsManager.KeyMappings[InputAction.LeftStrafe].MappedKey = KeyCode.None;
            SettingsManager.KeyMappings[InputAction.RightStrafe].MappedKey = KeyCode.None;
            SettingsManager.SaveKeySettings(SettingsManager.KeyMappings.Select(d => d.Value).ToList());
            InputManager.OnKeysUpdated();
        }

        private void EnableUserKeys()
        {
            SettingsManager.KeyMappings[InputAction.Forward].MappedKey = _userKeys[InputAction.Forward].MappedKey;
            SettingsManager.KeyMappings[InputAction.Backward].MappedKey = _userKeys[InputAction.Backward].MappedKey;
            SettingsManager.KeyMappings[InputAction.Jump].MappedKey = _userKeys[InputAction.Jump].MappedKey;
            SettingsManager.KeyMappings[InputAction.LeftStrafe].MappedKey = _userKeys[InputAction.LeftStrafe].MappedKey;
            SettingsManager.KeyMappings[InputAction.RightStrafe].MappedKey =
                _userKeys[InputAction.RightStrafe].MappedKey;
            SettingsManager.SaveKeySettings(SettingsManager.KeyMappings.Select(d => d.Value).ToList());
            InputManager.OnKeysUpdated();
        }

        private Dictionary<InputAction, KeyMap> Clone(Dictionary<InputAction, KeyMap> source)
        {
            var clone = new Dictionary<InputAction, KeyMap>(source.Count);
            foreach (var kvp in source) clone.Add(kvp.Key, new KeyMap(kvp.Value));
            return clone;
        }

        /* Note: we should probably create a hook registry of sorts
           where hook methods are stored */

        private void InstallHook()
        {
            if (HookManager.Instance["flyhook"] == null)
            {
                var ec = Entities.GetInstance().me.entitycontroller;

                var flags = BindingFlags.NonPublic | BindingFlags.Instance;

                HookManager.Instance.Add(new MethodHook("flyhook",
                    ec.GetType().GetMethod("ApplyGravity", flags),
                    GetType().GetMethod("ApplyGravity", flags)));
            }

            HookManager.Instance["flyhook"].Install();
        }

        private void UninstallHook()
        {
            HookManager.Instance["flyhook"]?.Uninstall();
        }

        private void ApplyGravity()
        {
            var ec = Entities.GetInstance().me.entitycontroller;
            var vs = ec.GetType().GetField("verticalSpeed", 
                BindingFlags.Instance | BindingFlags.NonPublic);

            float val = (float) vs.GetValue(ec);

            if (ec.IsGrounded())
            {
                vs.SetValue(ec, 0f);
                return;
            }

            val -= 20f * Time.deltaTime;
            vs.SetValue(ec, val);
        }
    }
}