namespace Styx.Botting.Commands.Map
{
    public class CommandInteract : Command
    {
        public CommandInteract()
        {
            Type = CmdType.Map;
            Cmd = (int) MapCommand.Interact;
            Npc = false;
        }

        public bool Npc { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
    }
}