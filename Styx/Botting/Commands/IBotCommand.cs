using System;

namespace Styx.Botting.Commands
{
    public interface IBotCommand
    {
        void Execute(IBotEngine instance);
        event Action ExecutionComplete;
    }
}