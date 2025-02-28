namespace Styx.Botting.Commands.Combat
{
    public class CommandKillAll : Command
    {
        public CommandKillAll()
        {
            Type = CmdType.Combat;
            Cmd = (int) CombatCommand.KillAll;
            Teleport = false;
            Skip = false;
        }

        public bool Teleport { get; set; }
        public bool Skip { get; set; }
    }
}