using System;
using System.Collections.Generic;

namespace StyxLauncher
{
    public class Assembler
    {
        private readonly List<byte> _asm = new List<byte>();

        private readonly IntPtr _origin;

        public Assembler(IntPtr origin)
        {
            _origin = origin;
        }

        public void Push(int arg)
        {
            _asm.Add(arg < 128 ? (byte)0x6A : (byte)0x68);
            _asm.AddRange(arg <= 255 ? new[] { (byte)arg } : BitConverter.GetBytes(arg));
        }

        public void PushEax()
        {
            _asm.Add(0x50);
        }

        public void Call(int arg)
        {
            _asm.Add(0xB8);
            _asm.AddRange(BitConverter.GetBytes(arg));
            _asm.AddRange(new byte[] { 0xFF, 0xD0 });
        }

        public void Add(int arg)
        {
            _asm.AddRange(new byte[] { 0x83, 0xC4 });
            _asm.AddRange(arg <= 255 ? new[] { (byte)arg } : BitConverter.GetBytes(arg));
        }

        public void Return()
        {
            _asm.Add(0xC3);
        }

        public byte[] Assemble() => _asm.ToArray();
    }
}
