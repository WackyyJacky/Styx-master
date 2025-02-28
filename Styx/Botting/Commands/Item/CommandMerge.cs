namespace Styx.Botting.Commands.Item
{
    public class CommandMerge : Command
    {
        public CommandMerge()
        {
            Type = CmdType.Merge;
            Cmd = (int) MergeCommand.Merge;
        }

        public int ShopId { get; set; }
        public int MergeId { get; set; }
    }
}