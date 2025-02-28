using System.Collections.Generic;
using Styx.Botting.Commands;

namespace Styx.Botting
{
    public class BotConfig
    {
        public List<Command> Commands { get; set; }
        public List<int> Quests { get; set; }
        public List<string> Loot { get; set; }
        public List<Spell> Spells { get; set; }
        public bool Relogin { get; set; }
        public string Server { get; set; }
        public int BotDelay { get; set; }
    }
}