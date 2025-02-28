using UnityEngine;

namespace Styx.Botting.Commands.Combat
{
    public class CommandRestTeleport : Command
    {
        public CommandRestTeleport()
        {
            Type = CmdType.Combat;
            Cmd = (int) CombatCommand.RestTeleport;
            Text = "Rest [Teleport]";
            Position = Position;
        }

        public Vector3 Position { get; set; }
    }
}