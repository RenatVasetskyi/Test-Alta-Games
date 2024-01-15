using Architecture.Services.Interfaces;
using Audio;
using Game.Data;
using Game.Interfaces;
using UnityEngine;
using Zenject;

namespace Game
{
    public class DestroyableBall : MonoBehaviour
    {
        private const float MovementDuration = 1.5f;

        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private SphereCollider _collider;
        
        [Space]
        
        [SerializeField] private LayerMask _obstacleLayer;

        private IAudioService _audioService;
        
        private Level _level;
        
        private LTDescr _movementTween;
        private LTDescr _rotationTween;

        [Inject]
        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }
        
        public void Initialize(Level level)
        {
            _level = level;
        }

        public void AddScale(Vector3 scale)
        {
            transform.localScale += scale;
        }

        public void Move(Vector3 to)
        {
            _movementTween = LeanTween.move(gameObject, to, MovementDuration)
                .setOnComplete(DestroyAndCheckIsHasObstaclesOnPath);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer == Layers.ObstacleLayerNumber)
            {
                if (_movementTween != null)
                {
                    StopMovement();
                    
                    DetectObstaclesAndHide();
                }
            }
        }

        private void StopMovement()
        {
            LeanTween.cancel(_movementTween.id);

            _rigidbody.velocity = Vector3.zero;
            _rigidbody.isKinematic = true;
        }

        private void DetectObstaclesAndHide()
        {
            float detectRadius = (_collider.radius * 2) * transform.localScale.y;
            
            RaycastHit[] obstacles = Physics.SphereCastAll(transform.position, detectRadius,
                transform.right * detectRadius, detectRadius, _obstacleLayer);

            foreach (RaycastHit obstacle in obstacles)
            {
                if (obstacle.collider.gameObject.TryGetComponent(out IHideable hideable)) 
                    hideable.Hide();                    
            }

            DestroyAndCheckIsHasObstaclesOnPath();
        }

        private void DestroyAndCheckIsHasObstaclesOnPath()
        {
            _level.CheckIsHasObstaclesOnPath();
            
            _audioService.PlaySfx(SfxType.BallDestroy);
            
            Destroy(gameObject);
        }
    }
}