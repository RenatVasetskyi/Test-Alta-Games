using System;
using UnityEngine;

namespace Game
{
    public class PathLine : MonoBehaviour
    {
        private const int MaxScalePercent = 100;

        [SerializeField] private LayerMask _obstacleLayer;
        [SerializeField] private BoxCollider _collider;
        
        private float _startScale;
        
        public void ReduceScale(float percentStep)
        {
            float currentPercent = transform.localScale.y * MaxScalePercent / _startScale;
            
            float scaleToReduce = (_startScale / MaxScalePercent) * percentStep;

            if (currentPercent > percentStep)
                transform.localScale -= new Vector3(0, 0, scaleToReduce);
        }
        
        public void CheckIsHasObstaclesOnPath()
        {
            RaycastHit[] obstacles = Physics.BoxCastAll(transform.position, transform.lossyScale, 
                Vector3.zero, transform.rotation, 0, _obstacleLayer);
            
            if (obstacles.Length == 0)
                Debug.Log("Win");
        }

        private void Awake()
        {
            _startScale = transform.localScale.z;
        }
    }
}