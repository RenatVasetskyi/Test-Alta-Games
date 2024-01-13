using System;
using UI.Game.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Game
{
    public class ScreenTouchReporter : MonoBehaviour, IPointerEnterHandler,
        IPointerExitHandler, IScreenTouchReporter
    {
        public event Action<bool> OnScreenTouched;

        private bool _isLocked = true;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_isLocked)
                return;
            
            OnScreenTouched?.Invoke(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnScreenTouched?.Invoke(false);
        }

        public void SetLockState(bool isLocked)
        {
            _isLocked = isLocked;
        }
    }
}