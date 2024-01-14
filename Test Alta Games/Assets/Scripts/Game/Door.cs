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
        
        [SerializeField] private LeanTweenType _openDoorEasing;

        private void OnTriggerEnter(Collider other)
        {
            Open();
        }

        private void Open()
        {
            LeanTween.rotateLocal(_door, new Vector3(_door.transform.rotation.x,
                    _openedDoorRotationY, _door.transform.rotation.z), _openDuration)
                .setEase(_openDoorEasing);
        }
    }
}