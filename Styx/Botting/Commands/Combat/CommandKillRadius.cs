using UnityEngine;

namespace Styx.Botting.Commands.Combat
{
    public class CommandKillRadius : Command
    {
        public CommandKillRadius()
        {
            Type = CmdType.Combat;
            Cmd = (int) CombatCommand.KillRadius;
        }

        public bool Teleport { get; set; }
        public Vector3 Position { get; set; }
        public float Radius { get; set; }
    }
}