using System.Collections;
using Architecture.Services.Interfaces;
using Data;
using UI.Game.Interfaces;
using UnityEngine;
using Zenject;

namespace Game
{
    public class Ball : MonoBehaviour
    {
        private const float ScalePercentStep = 1f;
        private const float TimeStep = 0.05f;

        private const int CriticalScalePercent = 10;
        private const int MaxScalePercent = 100;
        
        [SerializeField] private SphereCollider _collider;

        private GameSettings _gameSettings;
        private IBaseFactory _baseFactory;
        
        private IScreenTouchReporter _screenTouchReporter;
        private PathLine _pathLine;
        
        private DestroyableBall _destroyableBallPrefab;
        
        private bool _isScreenTouched;

        private float _startScale;

        private Transform _targetPoint;
        
        [Inject]
        public void Construct(GameSettings gameSettings, IBaseFactory baseFactory)
        {
            _gameSettings = gameSettings;
            _baseFactory = baseFactory;
        }
        
        public void Initialize(IScreenTouchReporter screenTouchReporter, PathLine pathLine, 
            Transform targetPoint)
        {
            _screenTouchReporter = screenTouchReporter;
            _pathLine = pathLine;
            _targetPoint = targetPoint;

            Subscribe();
        }

        private void Awake()
        {
            _startScale = transform.localScale.y;
        }

        private void OnDestroy()
        {
            UnSubscribe();
        }

        private async void CreateNewBall()
        { 
            DestroyableBall newBall = (await _baseFactory.CreateAddressableWithContainer
                (_gameSettings.DestroyableBall, transform.position + transform.right *
                    _collider.radius * 2, Quaternion.identity, transform.parent))
                .GetComponent<DestroyableBall>();
            
            newBall.Initialize(_pathLine);

            newBall.transform.localScale = Vector3.zero;
            
            StartCoroutine(ScaleBall(newBall));
        }

        private IEnumerator ScaleBall(DestroyableBall newBall)
        {
            while (_isScreenTouched)
            {
                float ballCurrentScalePercent = ReduceScale(newBall, ScalePercentStep);

                if (ballCurrentScalePercent < CriticalScalePercent)
                {
                    Debug.Log("Lose");
                    
                    Destroy(gameObject);
                }
                else
                {
                    _pathLine.ReduceScale(ScalePercentStep);
                }
                
                yield return new WaitForSeconds(TimeStep);
            }
            
            newBall.Move(_targetPoint.position);
        }

        private float ReduceScale(DestroyableBall newBall, float percentStep)
        {
            float currentPercent = transform.localScale.y * MaxScalePercent / _startScale;
            
            float scaleToReduce = (_startScale / MaxScalePercent) * percentStep;

            if (currentPercent > percentStep)
            {
                Vector3 scale = new Vector3(scaleToReduce, scaleToReduce, scaleToReduce);
                
                transform.localScale -= scale;
                
                newBall.AddScale(scale);
            }

            return currentPercent;
        }

        private void ScreenTouchHandler(bool isTouched)
        {
            _isScreenTouched = isTouched;
            
            if (isTouched)
                CreateNewBall();
        }
        
        private void Subscribe()
        {
            _screenTouchReporter.OnScreenTouched += ScreenTouchHandler;
        }

        private void UnSubscribe()
        {
            _screenTouchReporter.OnScreenTouched -= ScreenTouchHandler;
        }
    }
}