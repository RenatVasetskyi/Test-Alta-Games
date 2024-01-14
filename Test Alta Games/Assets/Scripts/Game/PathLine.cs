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
        
        public bool CheckIsHasObstaclesOnPath()
        {
            Collider[] colliders = Physics.OverlapBox(_collider.bounds.center,
                _collider.bounds.extents, Quaternion.identity, _obstacleLayer);

            return colliders.Length == 0;
        }

        private void Awake()
        {
            _startScale = transform.localScale.z;
        }
    }
}