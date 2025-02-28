using UnityEngine;

namespace Styx.Botting.Commands.Combat
{
    public class CommandKill : Command
    {
        public CommandKill()
        {
            Type = CmdType.Combat;
            Cmd = (int) CombatCommand.Kill;
            Skip = false;
        }

        public string Monster { get; set; }
        public bool Teleport { get; set; }
        public float Radius { get; internal set; }
        public Vector3 PlayerPos { get; set; }
        public int Amount { get; set; }
        public bool Skip { get; set; }
    }
}