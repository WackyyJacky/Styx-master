namespace Styx.Botting.Commands.Misc
{
    public class CommandSendPack : Command
    {
        public CommandSendPack()
        {
            Type = CmdType.Misc;
            Cmd = (int) MiscCommand.SendPacket;
        }

        public string Packet { get; set; }
    }
}