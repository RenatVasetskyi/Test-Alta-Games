using System;

namespace Game.Interfaces
{
    public interface IGameOverReporter
    {
        event Action OnWin;
        event Action OnLose;
        void SendLose();
    }
}