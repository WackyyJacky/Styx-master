using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Timers;
using Newtonsoft.Json;

namespace Styx.Tools.Packets
{
    public class Spammer
    {
        private int _index;
        private List<object[]> _requestParams; // The parameters for each request, passed to sendMessage

        private MethodInfo _sendMessage;

        private Timer _tmrSpam;

        private Spammer()
        {
        }

        public static Spammer Instance { get; } = new Spammer();
        public event Action<int> IndexChanged;

        public void Stop()
        {
            _tmrSpam.Stop();
        }

        public void Start(List<string> reqs, int spamDelay)
        {
            _tmrSpam = new Timer(spamDelay);
            _tmrSpam.Elapsed += tmrSpam_Elapsed;
            PrepareRequests(reqs);
            _sendMessage = Game.Instance.aec.GetType()
                .GetMethod("sendMessage", BindingFlags.NonPublic | BindingFlags.Instance);
            _tmrSpam.Start();
        }

        private void tmrSpam_Elapsed(object sender, EventArgs e)
        {
            if (_index >= _requestParams.Count) _index = 0;
            IndexChanged?.Invoke(_index);
            SendRequest(_index++);
        }

        private void PrepareRequests(List<string> reqs)
        {
            _requestParams = new List<object[]>(reqs.Count);
            foreach (var r in reqs)
            {
                var req = JsonConvert.DeserializeObject<Request>(r);
                _requestParams.Add(new object[] {Encoding.ASCII.GetBytes(r), req.type, req.cmd});
            }
        }

        private void SendRequest(int i)
        {
            _sendMessage.Invoke(Game.Instance.aec, _requestParams[i]);
        }
    }
}