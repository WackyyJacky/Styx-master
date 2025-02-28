namespace Styx.Botting.Commands.Misc
{
    public class CommandRestart : Command
    {
        public CommandRestart()
        {
            Type = CmdType.Misc;
            Cmd = (int) MiscCommand.Restart;
            Text = "Restart bot";
        }
    }
}