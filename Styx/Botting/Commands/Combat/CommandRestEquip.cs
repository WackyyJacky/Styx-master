namespace Styx.Botting.Commands.Combat
{
    public class CommandRestEquip : Command
    {
        public CommandRestEquip()
        {
            Type = CmdType.Combat;
            Cmd = (int) CombatCommand.RestEquip;
        }

        public string Item { get; set; }
    }
}