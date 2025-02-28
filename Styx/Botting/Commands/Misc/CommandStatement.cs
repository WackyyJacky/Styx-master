namespace Styx.Botting.Commands.Misc
{
    public class CommandStatement : Command
    {
        public CommandStatement()
        {
            Type = CmdType.Misc;
            Cmd = (int) MiscCommand.IfStatement;
        }

        public string Tag { get; set; }
        public int Tag2 { get; set; }
        public string Value1 { get; set; }
        public bool Loop { get; set; }
    }
}