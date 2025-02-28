namespace Styx.Botting.Commands.Quest
{
    public class CommandQuestComplete : Command
    {
        public CommandQuestComplete()
        {
            Type = CmdType.Quest;
            Cmd = (int) QuestCommand.Complete;
        }

        public int QuestId { get; set; }
    }
}