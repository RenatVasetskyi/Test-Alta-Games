using System;
using UnityEngine;

namespace Game
{
    public class PathLine : MonoBehaviour
    {
        private const int MaxScalePercent = 100;
        
        private float _startScale;
        
        public void ReduceScale(float percentStep)
        {
            float currentPercent = transform.localScale.y * MaxScalePercent / _startScale;
            
            float scaleToReduce = (_startScale / MaxScalePercent) * percentStep;

            if (currentPercent > percentStep)
                transform.localScale -= new Vector3(0, 0, scaleToReduce);
        }

        private void Awake()
        {
            _startScale = transform.localScale.z;
        }
    }
}