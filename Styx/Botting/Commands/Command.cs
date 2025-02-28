namespace Styx.Botting.Commands
{
    public class Command
    {
        public enum CmdType
        {
            Combat = 0,
            Map = 1,
            Quest = 2,
            Item = 3,
            Misc = 4,
            Buy = 11,
            Merge = 28
        }

        public enum CombatCommand
        {
            KillFor = 0,
            KillAll = 1,
            Kill = 2,
            RestWait = 3,
            RestEquip = 4,
            KillAllFor = 5,
            RestTeleport = 6,
            KillRadius = 7
        }

        public enum ItemCommand
        {
            Sell = 0,
            Buy = 1,
            Use = 2
        }

        public enum MapCommand
        {
            Join = 0,
            Waypoint = 1,
            MoveTo = 2,
            NextDungeon = 3,
            Interact = 4,
            MoveToMachine = 5,
            UseMachine = 6
        }

        public enum MergeCommand
        {
            Merge = 1,
            ClaimMerge = 3
        }

        public enum MiscCommand
        {
            Delay = 0,
            Restart = 1,
            Stop = 2,
            SendPacket = 3,
            IfStatement = 4,
            EndIfStatement = 5
        }

        public enum QuestCommand
        {
            Accept = 0,
            Complete = 1
        }

        public CmdType Type { get; set; }
        public int Cmd { get; set; }
        public string Text { get; set; }
    }
}