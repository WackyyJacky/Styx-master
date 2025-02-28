using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Styx.Core
{
    public class Mono
    {
        private readonly IntPtr _monoHandle;

        public Mono()
        {
            _monoHandle = Process.GetCurrentProcess().Modules.Cast<ProcessModule>()
                .First(m => m.ModuleName == "mono.dll").BaseAddress;
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, int dwSize,
            int flAllocationType = 0x00001000, int flProtect = 0x40);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, int dwFreeType = 0x4000);

        public IntPtr GetMethod(string assemblyName, string nameSpace, string className, string methodName,
            int methodParamCount)
        {
            IntPtr funcGetDomainAddr, funcAttachAddr, funcOpenAddr, funcImageAddr, funcClassAddr, funcMethodAddr;
            if ((funcGetDomainAddr = GetProcAddress(_monoHandle, "mono_get_root_domain")) == IntPtr.Zero)
                return IntPtr.Zero;
            if ((funcAttachAddr = GetProcAddress(_monoHandle, "mono_thread_attach")) == IntPtr.Zero)
                return IntPtr.Zero;
            if ((funcOpenAddr = GetProcAddress(_monoHandle, "mono_domain_assembly_open")) == IntPtr.Zero)
                return IntPtr.Zero;
            if ((funcImageAddr = GetProcAddress(_monoHandle, "mono_assembly_get_image")) == IntPtr.Zero)
                return IntPtr.Zero;
            if ((funcClassAddr = GetProcAddress(_monoHandle, "mono_class_from_name")) == IntPtr.Zero)
                return IntPtr.Zero;
            if ((funcMethodAddr = GetProcAddress(_monoHandle, "mono_class_get_method_from_name")) == IntPtr.Zero)
                return IntPtr.Zero;

            var funcGetDomain = GetDelegate<MonoGetRootDomain>(funcGetDomainAddr);
            var funcAttach = GetDelegate<MonoThreadAttach>(funcAttachAddr);
            var funcOpen = GetDelegate<MonoDomainAssemblyOpen>(funcOpenAddr);
            var funcImage = GetDelegate<MonoAssemblyGetImage>(funcImageAddr);
            var funcClass = GetDelegate<MonoClassFromName>(funcClassAddr);
            var funcMethod = GetDelegate<MonoClassGetMethodFromName>(funcMethodAddr);

            IntPtr domain, thread, assembly, image, klass;

            if ((domain = funcGetDomain()) == IntPtr.Zero)
                return IntPtr.Zero;
            if ((thread = funcAttach(domain)) == IntPtr.Zero)
                return IntPtr.Zero;
            if ((assembly = funcOpen(domain, assemblyName)) == IntPtr.Zero)
                return IntPtr.Zero;
            if ((image = funcImage(assembly)) == IntPtr.Zero)
                return IntPtr.Zero;
            if ((klass = funcClass(image, nameSpace, className)) == IntPtr.Zero)
                return IntPtr.Zero;

            return funcMethod(klass, methodName, methodParamCount);
        }

        private T GetDelegate<T>(IntPtr ptr)
        {
            return (T) (object) Marshal.GetDelegateForFunctionPointer(ptr, typeof(T));
        }

        private delegate IntPtr MonoGetRootDomain();

        private delegate IntPtr MonoThreadAttach(IntPtr domain);

        private delegate IntPtr MonoDomainAssemblyOpen(IntPtr domain, string name);

        private delegate IntPtr MonoAssemblyGetImage(IntPtr assembly);

        private delegate IntPtr MonoClassFromName(IntPtr image, string nameSpace, string klass);

        private delegate IntPtr MonoClassGetMethodFromName(IntPtr klass, string name, int paramcount);
    }
}