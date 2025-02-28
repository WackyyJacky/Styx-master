using System.Reflection;

namespace Styx.Tools.Hooking
{
    /// <summary>
    ///     Works by overwriting the first 3 instructions of a method with 3 NOP instructions (0x90) and then
    ///     a JMP instruction(0xE9) that unconditionally transfers the execution flow to the hook method.
    ///     The hook method needs to do whatever the original method did (unless the hook is intended
    ///     to stop the original code from executing). Reflection greatly simplifies that.
    /// </summary>
    public class MethodHook
    {
        private readonly byte[] _restore = new byte[8];

        public MethodHook(string name)
        {
            Name = name;
        }

        public MethodHook(string name, MethodInfo originalMethod, MethodInfo hookMethod)
        {
            Name = name;
            OriginalMethod = originalMethod;
            HookMethod = hookMethod;
        }

        public string Name { get; set; }
        public MethodInfo OriginalMethod { get; set; }
        public MethodInfo HookMethod { get; set; }

        public unsafe bool Install()
        {
            if (OriginalMethod != null && HookMethod != null)
            {
                var ptrTarget = (uint*) OriginalMethod.MethodHandle.GetFunctionPointer();
                var addrHook = (uint) HookMethod.MethodHandle.GetFunctionPointer();

                var body = (byte*) ptrTarget;

                for (var i = 0; i < 0x8; i++) _restore[i] = body[i];

                *ptrTarget = 0xE9909090; // nop, nop, nop, jmp
                *(ptrTarget + 0x1) = addrHook - (uint) ptrTarget - 0x8;
                HookManager.Instance.Add(this);
                return true;
            }

            return false;
        }

        public unsafe void Uninstall()
        {
            var ptrTarget = (byte*) OriginalMethod.MethodHandle.GetFunctionPointer();
            for (var i = 0; i < _restore.Length; i++) ptrTarget[i] = _restore[i];
            HookManager.Instance.Remove(this);
        }
    }
}