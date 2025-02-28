namespace Styx.Botting.Commands.Misc
{
    public class CommandDelay : Command
    {
        public CommandDelay()
        {
            Type = CmdType.Misc;
            Cmd = (int) MiscCommand.Delay;
        }

        public int Duration { get; set; }
    }
}