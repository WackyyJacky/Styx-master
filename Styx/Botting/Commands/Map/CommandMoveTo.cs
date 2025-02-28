namespace Styx.Botting.Commands.Map
{
    public class CommandMoveTo : Command
    {
        public CommandMoveTo()
        {
            Type = CmdType.Map;
            Cmd = (int) MapCommand.MoveTo;
        }

        public string Npc { get; set; }
        public bool Teleport { get; set; }
    }
}