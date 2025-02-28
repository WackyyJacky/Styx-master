using System;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Styx.Tools.Hooking;

namespace Styx.Tools.Packets
{
    public class Sniffer
    {
        private static MethodInfo _sendMessage;

        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };

        public static Sniffer Instance { get; } = new Sniffer();

        public bool IsRunning { get; set; }

        public event Action<string> PacketReceived;
        public event Action<Request> BeforePacketSent;
        public event Action<string> PacketSent;

        public void Stop()
        {
            AEC.getInstance().ResponseReceived -= ResponseReceived;
            HookManager.Instance["PacketSniffer"]?.Uninstall();
            IsRunning = false;
        }

        public void Start()
        {
            if (!IsRunning)
            {
                _sendMessage = AEC.getInstance().GetType()
                    .GetMethod("sendMessage", BindingFlags.NonPublic | BindingFlags.Instance);
                AEC.getInstance().ResponseReceived += ResponseReceived;
                InstallHook();
                IsRunning = true;
            }
        }

        private void InstallHook()
        {
            var original = AEC.getInstance().GetType()
                .GetMethod("sendRequest", BindingFlags.Public | BindingFlags.Instance);
            var hookMethod = GetType().GetMethod("SendRequest", BindingFlags.Public | BindingFlags.Instance);

            var hook = new MethodHook("PacketSniffer", original, hookMethod);
            hook.Install();
        }

        private void ResponseReceived(Response r)
        {
            PacketReceived?.Invoke(JsonConvert.SerializeObject(r));
        }

        public void SendRequest(Request r)
        {
            Instance.BeforePacketSent?.Invoke(r);
            var s = JsonConvert.SerializeObject(r, Formatting.None, SerializerSettings);
            Instance.SendMessage(Encoding.ASCII.GetBytes(s), r.type, r.cmd);
            Instance.PacketSent?.Invoke(s);
        }

        private void SendMessage(byte[] message, byte type = 255, byte cmd = 255)
        {
            _sendMessage?.Invoke(AEC.getInstance(), new object[] {message, type, cmd});
        }
    }
}