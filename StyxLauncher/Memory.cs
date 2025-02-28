using System;
using System.Collections.Generic;

namespace StyxLauncher
{
    public class Memory : IDisposable
    {
        private readonly IntPtr _handle;

        private readonly Dictionary<IntPtr, int> _allocations = new Dictionary<IntPtr, int>();

        public Memory(IntPtr processHandle)
        {
            _handle = processHandle;
        }

        public IntPtr AllocateAndWrite(byte[] data)
        {
            IntPtr addr = Allocate(data);
            Write(addr, data);
            return addr;
        }

        public IntPtr Allocate(byte[] data)
        {
            IntPtr addr =
                UnsafeNativeMethods.VirtualAllocEx(_handle, IntPtr.Zero, data.Length,
                    AllocationType.MEM_COMMIT, MemoryProtection.PAGE_EXECUTE_READWRITE);
            _allocations.Add(addr, data.Length);
            return addr;
        }

        public void Write(IntPtr addr, byte[] data)
        {
            UnsafeNativeMethods.WriteProcessMemory(_handle, addr, data, data.Length, out _);
        }

        public void Dispose()
        {
            foreach (var kvp in _allocations)
                UnsafeNativeMethods.VirtualFreeEx(_handle,
                    kvp.Key, kvp.Value, MemoryFreeType.MEM_DECOMMIT);
        }
    }
}
