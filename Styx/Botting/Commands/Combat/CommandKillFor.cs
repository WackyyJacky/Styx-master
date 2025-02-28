namespace Styx.Botting.Commands.Combat
{
    public class CommandKillFor : Command
    {
        public CommandKillFor()
        {
            Type = CmdType.Combat;
            Cmd = (int) CombatCommand.KillFor;
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