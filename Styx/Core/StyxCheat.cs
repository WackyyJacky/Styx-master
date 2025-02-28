using System.Threading;
using Styx.Tools;
using Styx.UI;
using UnityEngine;
using Application = System.Windows.Forms.Application;

namespace Styx.Core
{
    // TODO: Refactor
    public class StyxCheat : MonoBehaviour
    {
        public static StyxCheat Instance;
        public Access Access;

        public ChatCommand ChatCmd;
        public FlyCheat FlyCheat;

        private void Awake()
        {
            new Thread(() => Application.Run(Root.Instance)).Start();
            ChatCmd = new ChatCommand();
            FlyCheat = new FlyCheat();
            Access = new Access();
        }

        private void Update()
        {
            if (ChatCmd == null) ChatCmd = new ChatCommand();
            if (Access == null) Access = new Access();
            if (FlyCheat == null) FlyCheat = new FlyCheat();
            foreach (var kb in KeyBind.ActiveBinds)
                if (Input.GetKeyDown(kb.Key))
                    KeyBind.Actions[kb.ActionIndex]();

            ChatCmd.Subscribe();
            Access.Subscribe();
            if (FlyCheat.IsActive)
            {
                if (Input.GetKey(FlyCheat.ForwardKey))
                    Entities.GetInstance().me.wrapper.transform.Translate(Vector3.forward * 15f * Time.deltaTime);
                if (Input.GetKey(FlyCheat.BackwardKey))
                    Entities.GetInstance().me.wrapper.transform.Translate(Vector3.down * 15f * Time.deltaTime);
                if (Input.GetKey(FlyCheat.JumpKey))
                    Entities.GetInstance().me.wrapper.transform.Translate(Vector3.up * 15f * Time.deltaTime);
                if (Input.GetKey(FlyCheat.RightStrafeKey))
                    Entities.GetInstance().me.wrapper.transform.Translate(Vector3.right * 15f * Time.deltaTime);
                if (Input.GetKey(FlyCheat.LeftStrafeKey))
                    Entities.GetInstance().me.wrapper.transform.Translate(Vector3.left * 15f * Time.deltaTime);
            }

            if (Entities.GetInstance() != null)
                UpdateStaffCount();
        }

        private void UpdateStaffCount()
        {
            var staff = World.Staff;
            if (staff != null)
                Root.Instance.SetStaffCount(staff.Count);
        }

        private void OnApplicationQuit()
        {
            Loader.Unload();
        }
    }
}