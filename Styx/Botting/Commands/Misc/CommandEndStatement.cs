namespace Styx.Botting.Commands.Combat
{
    public class CommandEndStatement : Command
    {
        public CommandEndStatement()
        {
            Type = CmdType.Misc;
            Cmd = (int) MiscCommand.EndIfStatement;
            Text = "EndIF";
        }
    }
}