using UnityEngine;

namespace Game
{
    public class DestroyableBall : MonoBehaviour
    {
        private const int MaxScalePercent = 100;
        
        private float _startScale;
        
        public void AddScale(float percentStep)
        {
            float currentPercent = transform.localScale.y * MaxScalePercent / _startScale;
            
            float scaleToAdd = (_startScale / MaxScalePercent) * percentStep;

            if (currentPercent + percentStep < MaxScalePercent)
                transform.localScale += new Vector3(scaleToAdd, scaleToAdd, scaleToAdd);
        }

        private void Awake()
        {
            _startScale = transform.localScale.y;
        }
    }
}