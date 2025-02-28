namespace Styx.Botting.Commands.Map
{
    public class CommandUseMachine : Command
    {
        public CommandUseMachine()
        {
            Type = CmdType.Map;
            Cmd = (int) MapCommand.UseMachine;
        }

        public string Name { get; set; }
        public int Id { get; set; }
    }
}