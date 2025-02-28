using UnityEngine;

namespace Styx.Botting.Commands.Map
{
    public class CommandWaypoint : Command
    {
        public CommandWaypoint()
        {
            Type = CmdType.Map;
            Cmd = (int) MapCommand.Waypoint;
        }

        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public bool Teleport { get; set; }
    }
}