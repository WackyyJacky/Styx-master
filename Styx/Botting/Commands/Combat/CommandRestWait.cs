namespace Styx.Botting.Commands.Combat
{
    public class CommandRestWait : Command
    {
        public CommandRestWait()
        {
            Type = CmdType.Combat;
            Cmd = (int) CombatCommand.RestWait;
            Text = "Rest [Wait]";
        }
    }
}