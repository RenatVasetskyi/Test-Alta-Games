using System.Collections;
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

        private PathLine _pathLine;
        
        private LTDescr _movementTween;
        private LTDescr _rotationTween;

        public void Initialize(PathLine pathLine)
        {
            _pathLine = pathLine;
        }
        
        public void AddScale(Vector3 scale)
        {
            transform.localScale += scale;
        }

        public void Move(Vector3 to)
        {
            _movementTween = LeanTween.move(gameObject, to, MovementDuration);
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
            float detectRadius = (_collider.radius * 2 ) * transform.localScale.y;
            
            RaycastHit[] obstacles = Physics.SphereCastAll(transform.position, detectRadius,
                transform.right * detectRadius, detectRadius, _obstacleLayer);

            foreach (RaycastHit obstacle in obstacles)
            {
                IDestroyableObstacle destroyableObstacle = obstacle.collider.GetComponent<IDestroyableObstacle>();
                
                destroyableObstacle.Destroy();
            }
            
            StartCoroutine(AAAA());
        }

        private IEnumerator AAAA()
        {
            yield return new WaitForSeconds(1);
            
            _pathLine.CheckIsHasObstaclesOnPath();
            
            Destroy(gameObject);
        }
    }
}