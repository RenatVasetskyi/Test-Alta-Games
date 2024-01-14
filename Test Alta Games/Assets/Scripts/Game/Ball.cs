using System.Collections;
using UI.Game.Interfaces;
using UnityEngine;

namespace Game
{
    public class Ball : MonoBehaviour
    {
        private const float ScalePercentStep = 1f;
        private const float TimeStep = 0.05f;

        private const int CriticalScalePercent = 10;
        private const int MaxScalePercent = 100;
        
        [SerializeField] private SphereCollider _collider;

        private IScreenTouchReporter _screenTouchReporter;
        private Level _level;
        
        private DestroyableBall _destroyableBallPrefab;
        
        private bool _isScreenTouched;

        private float _startScale;
        
        public void Initialize(IScreenTouchReporter screenTouchReporter, Level level)
        {
            _screenTouchReporter = screenTouchReporter;
            _level = level;

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
            DestroyableBall destroyableBall = await _level.CreateDestroyableBall(transform, _collider.radius * 2);
            
            StartCoroutine(ScaleBall(destroyableBall));
        }

        private IEnumerator ScaleBall(DestroyableBall newBall)
        {
            while (_isScreenTouched)
            {
                float ballCurrentScalePercent = ReduceScale(ScalePercentStep);

                if (ballCurrentScalePercent < CriticalScalePercent)
                {
                    _level.SendLose();
                    
                    Destroy(gameObject);
                }
                
                yield return new WaitForSeconds(TimeStep);
            }
            
            newBall.Move(_level.TargetPoint.position);
        }

        private float ReduceScale(float percentStep)
        {
            float currentPercent = transform.localScale.y * MaxScalePercent / _startScale;
            
            float scaleToReduce = (_startScale / MaxScalePercent) * percentStep;

            if (currentPercent > percentStep)
            {
                Vector3 scale = new Vector3(scaleToReduce, scaleToReduce, scaleToReduce);
                
                transform.localScale -= scale;
                
                _level.ScaleObjects(percentStep, scale);
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