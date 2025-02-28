namespace Styx.Botting.Commands.Item
{
    public class CommandClaimMerge : Command
    {
        public CommandClaimMerge()
        {
            Type = CmdType.Merge;
            Cmd = (int) MergeCommand.ClaimMerge;
        }

        public int MergeId { get; set; }
    }
}