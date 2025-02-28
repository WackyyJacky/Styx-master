using System.Collections.Generic;
using System.Linq;

namespace Styx.Tools.Hooking
{
    public class HookManager
    {
        private readonly List<MethodHook> _hooks; // Hooks are automatically added/removed when installed/uninstalled

        private HookManager()
        {
            _hooks = new List<MethodHook>();
        }

        public static HookManager Instance { get; } = new HookManager();

        public MethodHook this[string name] => _hooks.FirstOrDefault(h => h.Name == name);

        public bool Add(MethodHook hook)
        {
            if (this[hook.Name] != null) return false;
            _hooks.Add(hook);
            return true;
        }

        public bool Remove(MethodHook hook)
        {
            return _hooks.Remove(hook);
        }

        public bool UninstallAllHooks()
        {
            _hooks.ForEach(h => h.Uninstall());
            return _hooks.Count <= 0;
        }
    }
}