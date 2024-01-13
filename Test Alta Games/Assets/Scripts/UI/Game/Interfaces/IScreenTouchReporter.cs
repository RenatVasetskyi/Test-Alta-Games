using System;

namespace UI.Game.Interfaces
{
    public interface IScreenTouchReporter
    {
        event Action<bool> OnScreenTouched;
    }
}