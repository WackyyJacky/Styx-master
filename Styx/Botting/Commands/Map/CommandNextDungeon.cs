namespace Styx.Botting.Commands.Map
{
    public class CommandNextDungeon : Command
    {
        public CommandNextDungeon()
        {
            Type = CmdType.Map;
            Cmd = (int) MapCommand.NextDungeon;
            Teleport = true;
        }

        public bool Teleport { get; set; }
    }
}