using Game.Interfaces;
using UnityEngine;

namespace Game
{
    public class DestroyableBall : MonoBehaviour
    {
        private const int MaxScalePercent = 100;
        
        private const float MovementDuration = 2.5f;

        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private SphereCollider _collider;
        
        [Space]
        
        [SerializeField] private LayerMask _obstacleLayer;
        
        private float _startScale;

        private LTDescr _movementTween;
        private LTDescr _rotationTween;
        
        public void AddScale(float percentStep)
        {
            float currentPercent = transform.localScale.y * MaxScalePercent / _startScale;
            
            float scaleToAdd = (_startScale / MaxScalePercent) * percentStep;

            if (currentPercent + percentStep < MaxScalePercent)
                transform.localScale += new Vector3(scaleToAdd, scaleToAdd, scaleToAdd);
        }

        public void Move(Vector3 to)
        {
            _movementTween = LeanTween.move(gameObject, to, MovementDuration);
        }
        
        private void Awake()
        {
            _startScale = transform.localScale.y;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.TryGetComponent(out IDestroyableObstacle obstacle))
            {
                if (_movementTween != null)
                {
                    StopMovement();
                    
                    DetectObstaclesToDestroy();
                }
            }
        }

        private void StopMovement()
        {
            LeanTween.cancel(_movementTween.id);

            _rigidbody.velocity = Vector3.zero;
            _rigidbody.isKinematic = true;
        }

        private void DetectObstaclesToDestroy()
        {
            float radius = _collider.radius * transform.localScale.y;
            
            RaycastHit[] obstacles = Physics.SphereCastAll(transform.position, radius,
                transform.forward, radius, _obstacleLayer);

            foreach (RaycastHit obstacle in obstacles)
                obstacle.transform.position = new Vector3(obstacle.transform.position.x, 5, obstacle.transform.position.z);
        }
    }
}