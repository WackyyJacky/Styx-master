using System;

namespace Styx.Botting
{
    public interface IBotEngine
    {
        bool IsRunning { get; set; }
        BotConfig Configuration { get; set; }
        event Action<bool> IsRunningChanged;
        event Action<int> IndexChanged;
    }
}