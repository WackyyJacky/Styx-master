namespace Styx.Botting.Commands.Map
{
    public class CommandJoin : Command
    {
        public CommandJoin()
        {
            Type = CmdType.Map;
            Cmd = (int) MapCommand.Join;
        }

        public int MapId { get; set; }
        public bool RequestPrivateInstance { get; set; }
    }
}