using UnityEngine;

namespace Game
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private GameObject _door;
        
        [Space]
        
        [SerializeField] private int _openedDoorRotationY;
        [SerializeField] private float _openDuration;
        
        [Space]
        
        [SerializeField] private int _closedDoorRotationY;
        [SerializeField] private float _closeDuration;

        [Space] 
        
        [SerializeField] private LeanTweenType _openDoorEasing;
        [SerializeField] private LeanTweenType _closeDoorEasing;

        private LTDescr _openTween;
        private LTDescr _closeTween;
        
        private void OnTriggerEnter(Collider other)
        {
            Open();
        }

        private void OnTriggerExit(Collider other)
        {
            Close();
        }

        private void Open()
        {
            if (_closeTween != null)
                LeanTween.cancel(_closeTween.id);
            
            _openTween = LeanTween.rotateY(_door, _openedDoorRotationY, _openDuration)
                .setEase(_openDoorEasing);
        }

        private void Close()
        {
            if (_openTween != null)
                LeanTween.cancel(_openTween.id);
            
            _closeTween = LeanTween.rotateY(_door, _closedDoorRotationY, _closeDuration)
                .setEase(_closeDoorEasing);
        }
    }
}