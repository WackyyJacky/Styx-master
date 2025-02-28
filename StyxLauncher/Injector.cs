using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StyxLauncher
{
    public class Injector
    {
        private readonly IntPtr _monoHandle;

        private readonly IntPtr _handle;

        private readonly Memory _allocator;

        private byte[] _assembly;

        private IntPtr _assemblyPtr;

        private IntPtr _assemblyNamePtr;

        private IntPtr _namespacePtr;

        private IntPtr _classNamePtr;

        private IntPtr _methodNamePtr;


        private readonly Dictionary<string, IntPtr> _exports = new Dictionary<string, IntPtr>
        {
            {"mono_get_root_domain", IntPtr.Zero },
            {"mono_thread_attach", IntPtr.Zero },
            {"mono_image_open_from_data", IntPtr.Zero },
            {"mono_assembly_load_from_full", IntPtr.Zero },
            {"mono_assembly_get_image", IntPtr.Zero },
            {"mono_class_from_name", IntPtr.Zero },
            {"mono_class_get_method_from_name", IntPtr.Zero },
            {"mono_runtime_invoke", IntPtr.Zero }
        };

        public Injector(IntPtr processHandle, IntPtr monoModule)
        {
            _monoHandle = monoModule;
            _handle = processHandle;
            _allocator = new Memory(_handle);
        }

        public bool Inject(byte[] data, string @namespace, string klass, string method)
        {
            foreach (string func in _exports.Keys.ToArray())
                _exports[func] = UnsafeNativeMethods.GetProcAddress(_monoHandle, func);

            var invalid = _exports.Where(e => e.Value == IntPtr.Zero).ToArray();

            if (invalid.Any())
            {
                throw new ApplicationException(
                    string.Join("\r\n", invalid.Select(i => $"{i.Key} = 0")));
            }

            _assembly = data;
            _assemblyPtr = _allocator.AllocateAndWrite(data);
            _assemblyNamePtr = _allocator.AllocateAndWrite(new byte[1]);
            _namespacePtr = _allocator.AllocateAndWrite(Encoding.UTF8.GetBytes(@namespace));
            _classNamePtr = _allocator.AllocateAndWrite(Encoding.UTF8.GetBytes(klass));
            _methodNamePtr = _allocator.AllocateAndWrite(Encoding.UTF8.GetBytes(method));

            byte[] partial = Assemble(IntPtr.Zero);

            IntPtr cave = _allocator.Allocate(partial);

            byte[] full = Assemble(cave);

            _allocator.Write(cave, full);

            int result = (int)GetThreadReturnValue(
                UnsafeNativeMethods.CreateRemoteThread(
                    _handle, IntPtr.Zero, 0, cave, IntPtr.Zero, 0, out _));

            _allocator.Dispose();

            return result == 0;
        }

        private byte[] Assemble(IntPtr origin)
        {
            Assembler asm = new Assembler(origin);

            asm.Call((int)_exports["mono_get_root_domain"]);

            asm.PushEax();
            asm.Call((int)_exports["mono_thread_attach"]);
            asm.Add(4);

            asm.Push(0);
            asm.Push(1);
            asm.Push(_assembly.Length);
            asm.Push((int)_assemblyPtr);
            asm.Call((int)_exports["mono_image_open_from_data"]);
            asm.Add(16);

            asm.Push(0);
            asm.Push(0);
            asm.Push((int)_assemblyNamePtr);
            asm.PushEax();
            asm.Call((int)_exports["mono_assembly_load_from_full"]);
            asm.Add(16);

            asm.PushEax();
            asm.Call((int)_exports["mono_assembly_get_image"]);
            asm.Add(4);

            asm.Push((int)_classNamePtr);
            asm.Push((int)_namespacePtr);
            asm.PushEax();
            asm.Call((int)_exports["mono_class_from_name"]);
            asm.Add(12);

            asm.Push(0);
            asm.Push((int)_methodNamePtr);
            asm.PushEax();
            asm.Call((int)_exports["mono_class_get_method_from_name"]);
            asm.Add(12);

            asm.Push(0);
            asm.Push(0);
            asm.Push(0);
            asm.PushEax();
            asm.Call((int)_exports["mono_runtime_invoke"]);
            asm.Add(16);
            asm.Return();

            return asm.Assemble();
        }

        private IntPtr GetThreadReturnValue(IntPtr hThread)
        {
            UnsafeNativeMethods.WaitForSingleObject(hThread, -1);
            UnsafeNativeMethods.GetExitCodeThread(hThread, out IntPtr exitCode);
            return exitCode;
        }
    }
}