namespace Styx.Botting.Commands.Item
{
    public class CommandSell : Command
    {
        public CommandSell()
        {
            Type = CmdType.Item;
            Cmd = (int) ItemCommand.Sell;
        }

        public string Item { get; set; }
        public int Quantity { get; set; }
    }
}