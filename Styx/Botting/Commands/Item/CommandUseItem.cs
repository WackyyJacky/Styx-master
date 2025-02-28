namespace Styx.Botting.Commands.Item
{
    public class CommandUseItem : Command
    {
        public CommandUseItem()
        {
            Type = CmdType.Item;
            Cmd = (int) ItemCommand.Use;
        }

        public string Item { get; set; }
    }
}