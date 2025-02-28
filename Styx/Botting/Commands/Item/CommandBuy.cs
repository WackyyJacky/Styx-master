namespace Styx.Botting.Commands.Item
{
    public class CommandBuy : Command
    {
        public CommandBuy()
        {
            Type = CmdType.Buy;
            Cmd = (int) ItemCommand.Buy;
        }

        public int ItemId { get; set; }
        public int ShopId { get; set; }
        public short ItemQty { get; set; }
    }
}