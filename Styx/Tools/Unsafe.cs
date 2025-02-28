using System;
using System.Reflection;

namespace Styx.Tools
{
    public static class Unsafe
    {
        private const int JumpHackOffset = 0x2A;
        private const int JumpHeightOffset = 0xC;
        private static byte[] _originalJumpInstruction;

        private static unsafe void StoreJumpInstruction(byte* instruction)
        {
            _originalJumpInstruction = new[]
            {
                instruction[0],
                instruction[1],
                instruction[2],
                instruction[3],
                instruction[4]
            };
        }

        public static bool ToggleJumpHack(out string message)
        {
            var onJumped = Entities.GetInstance()?.me?.entitycontroller?.GetType()
                .GetMethod("OnJumped", BindingFlags.NonPublic | BindingFlags.Instance);
            if (onJumped != null)
            {
                IntPtr address;
                if ((address = (IntPtr) ((int) onJumped.MethodHandle.GetFunctionPointer() + JumpHackOffset)) !=
                    (IntPtr) JumpHackOffset)
                    unsafe
                    {
                        var instruction = (byte*) address;
                        if (_originalJumpInstruction == null) StoreJumpInstruction(instruction);
                        if (instruction[0] == 0x90)
                        {
                            instruction[0] = _originalJumpInstruction[0];
                            instruction[1] = _originalJumpInstruction[1];
                            instruction[2] = _originalJumpInstruction[2];
                            instruction[3] = _originalJumpInstruction[3];
                            instruction[4] = _originalJumpInstruction[4];
                            message = "Jump hack disabled";
                        }
                        else
                        {
                            for (var i = 0; i < 5; i++) instruction[i] = 0x90;
                            message = "Jump hack enabled";
                        }

                        return true;
                    }
            }

            message = "Jump hack failed";
            return false;
        }

        public static bool ToggleJumpHack()
        {
            var onJumped = Entities.GetInstance()?.me?.entitycontroller?.GetType()
                .GetMethod("OnJumped", BindingFlags.NonPublic | BindingFlags.Instance);
            if (onJumped != null)
            {
                IntPtr address;
                if ((address = (IntPtr) ((int) onJumped.MethodHandle.GetFunctionPointer() + JumpHackOffset)) !=
                    (IntPtr) JumpHackOffset)
                    unsafe
                    {
                        var instruction = (byte*) address;
                        if (_originalJumpInstruction == null) StoreJumpInstruction(instruction);
                        if (instruction[0] == 0x90)
                        {
                            instruction[0] = _originalJumpInstruction[0];
                            instruction[1] = _originalJumpInstruction[1];
                            instruction[2] = _originalJumpInstruction[2];
                            instruction[3] = _originalJumpInstruction[3];
                            instruction[4] = _originalJumpInstruction[4];
                        }
                        else
                        {
                            for (var i = 0; i < 5; i++) instruction[i] = 0x90;
                        }

                        return true;
                    }
            }

            return false;
        }

        public static bool SetJumpHeight(float height, out string message)
        {
            var jump = Entities.GetInstance()?.me?.entitycontroller?.GetType()
                .GetMethod("Jump", BindingFlags.NonPublic | BindingFlags.Instance);

            if (jump != null)
            {
                IntPtr address;
                if ((address = (IntPtr) ((int) jump.MethodHandle.GetFunctionPointer() + JumpHeightOffset)) !=
                    (IntPtr) JumpHeightOffset)
                    unsafe
                    {
                        var instruction = (byte*) address;
                        var ptr = (IntPtr) BitConverter.ToInt32(new[]
                        {
                            instruction[0],
                            instruction[1],
                            instruction[2],
                            instruction[3]
                        }, 0);
                        var jumpHeight = (float*) ptr;
                        *jumpHeight = height;
                        message = $"Jump height set to {height}";
                        return true;
                    }
            }

            message = "Jump height failure";
            return false;
        }
    }
}