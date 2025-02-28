namespace Styx.Botting.Commands.Combat
{
    public class CommandKillAllFor : Command
    {
        public CommandKillAllFor()
        {
            Type = CmdType.Combat;
            Cmd = (int) CombatCommand.KillAllFor;
            Text = "Kill all monsters for: " + ItemName + " (" + ItemQuantity + ")";
            Skip = false;
        }

        public bool IsLoot { get; set; }
        public string Monster { get; set; }
        public string ItemName { get; set; }
        public string ItemQuantity { get; set; }
        public bool Teleport { get; set; }
        public bool ItemStop { get; set; }
        public bool Skip { get; set; }
    }
}