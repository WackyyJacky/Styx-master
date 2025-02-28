namespace Styx.Botting.Commands.Combat
{
    public class CommandMoveToMachine : Command
    {
        public CommandMoveToMachine()
        {
            Type = CmdType.Map;
            Cmd = (int) MapCommand.MoveToMachine;
        }

        public string Machine { get; set; }
        public bool Teleport { get; set; }
        public int MachineId { get; set; }
    }
}