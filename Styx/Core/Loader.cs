using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Styx.Tools.Hooking;
using Styx.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Styx.Core
{
    public static class Loader
    {
        public static GameObject G;

        public static void Load()
        {
            HookTrackingMethods();
            G = new GameObject();
            StyxCheat.Instance = G.AddComponent<StyxCheat>();
            Object.DontDestroyOnLoad(G);
        }

        public static void Unload()
        {
            BotManager.Instance.Dispose();
            Grabbers.Instance.Dispose();
            KeyBinds.Instance.Dispose();
            Loaders.Instance.Dispose();
            Sniffer.Instance.Dispose();
            Spammer.Instance.Dispose();
            Staff.Instance.Dispose();
            Root.Instance.ResetUi();
            Root.Instance.Dispose();
            Object.Destroy(G);
        }

        private static void HookTrackingMethods()
        {
            var flags = BindingFlags.Public | BindingFlags.Static;

            var mono = new Mono();

            while (mono.GetMethod("Assembly-CSharp", "\0", "DeviceTracking", "RecordDeviceEvent", 1) == IntPtr.Zero)
                Thread.Sleep(1);
            while (mono.GetMethod("Assembly-CSharp", "\0", "DeviceTracking", "RecordDeviceData", 0) == IntPtr.Zero)
                Thread.Sleep(1);
            while (mono.GetMethod("Assembly-CSharp", "\0", "UserTracking", "RecordUserEvent", 1) == IntPtr.Zero)
                Thread.Sleep(1);

            var recordDeviceData = typeof(DeviceTracking).GetMethod("RecordDeviceData", flags);
            var recordDeviceEvent = typeof(DeviceTracking).GetMethod("RecordDeviceEvent", flags);
            var recordUserEvent = typeof(UserTracking).GetMethod("RecordUserEvent", flags);

            var recordDeviceDataHook = typeof(Loader).GetMethod("RecordDeviceData", flags);
            var recordDeviceEventHook = typeof(Loader).GetMethod("RecordDeviceEvent", flags);
            var recordUserEventHook = typeof(Loader).GetMethod("RecordUserEvent", flags);

            var deviceDataHook = new MethodHook("RecordDeviceData", recordDeviceData, recordDeviceDataHook);
            var deviceEventHook = new MethodHook("RecordDeviceEvent", recordDeviceEvent, recordDeviceEventHook);
            var userEventHook = new MethodHook("RecordUserEvent", recordUserEvent, recordUserEventHook);

            deviceDataHook.Install();
            deviceEventHook.Install();
            userEventHook.Install();
        }

        // Assembly-CSharp -> DeviceTracking
        public static void RecordDeviceEvent(DeviceTracking.DeviceEvent deviceEvent) { }

        // Assembly-CSharp -> DeviceTracking
        public static void RecordDeviceData() { }

        public static void RecordUserEvent() { }

        // Assembly-CSharp -> DeviceTracking
        private static string TrimBrackets(string msg)
        {
            return msg.Replace("<", string.Empty).Replace(">", string.Empty);
        }

        private static string GenerateRandomString()
        {
            var chars = "abcdefghijklmnopqrstuvwxyz1234567890".ToCharArray();
            var data = new byte[1];
            var crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            data = new byte[40];
            crypto.GetNonZeroBytes(data);
            var result = new StringBuilder(40);
            foreach (var b in data) result.Append(chars[b % chars.Length]);
            return result.ToString();
        }
    }
}