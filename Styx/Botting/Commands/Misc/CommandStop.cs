namespace Styx.Botting.Commands.Misc
{
    public class CommandStop : Command
    {
        public CommandStop()
        {
            Type = CmdType.Misc;
            Cmd = (int) MiscCommand.Stop;
            Text = "Stop bot";
        }
    }
}