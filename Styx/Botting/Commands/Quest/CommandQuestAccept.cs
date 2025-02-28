namespace Styx.Botting.Commands.Quest
{
    public class CommandQuestAccept : Command
    {
        public CommandQuestAccept()
        {
            Type = CmdType.Quest;
            Cmd = (int) QuestCommand.Accept;
        }

        public int QuestId { get; set; }
    }
}