using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UI.Game.Interfaces;
using UnityEngine;

namespace Game
{
    public class Ball : MonoBehaviour
    {
        private const int JumpsCount = 12;
        private const float JumpHeight = 1.5f;
        private const float MoveToDoorsDuration = 6f;
        
        private const float ScalePercentStep = 1f;
        private const float TimeStep = 0.05f;

        private const int CriticalScalePercent = 10;
        private const int MaxScalePercent = 100;
        
        [SerializeField] private SphereCollider _collider;

        [Space]
        
        [SerializeField] private Ease _jumpEasing; 
        
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

        private void MoveToDoors()
        {
            float distanceBetweenPoints = CalculateDistanceToDoors
                (out List<Vector3> movementPoints, out float currentDistance);

            for (int i = 0; i < JumpsCount; i++)
            {
                Vector3 pointToMove;

                if (i % 2 == 0)
                {
                    pointToMove = transform.position + (transform
                        .right * currentDistance) + new Vector3(0, JumpHeight, 0);
                }
                else
                {
                    pointToMove = transform.position + (transform.right * currentDistance);
                }

                movementPoints.Add(pointToMove);

                currentDistance += distanceBetweenPoints;
            }
            
            transform.DOPath(movementPoints.ToArray(), MoveToDoorsDuration)
                .SetEase(_jumpEasing);
        }

        private float CalculateDistanceToDoors(out List<Vector3> movementPoints, out float currentDistance)
        {
            float distance = Vector3.Distance(transform.position, _level.TargetPoint.position);

            float distanceBetweenPoints = distance / JumpsCount;

            movementPoints = new();

            currentDistance = distanceBetweenPoints;
            
            return distanceBetweenPoints;
        }

        private void Subscribe()
        {
            _screenTouchReporter.OnScreenTouched += ScreenTouchHandler;
            _level.OnWin += MoveToDoors;
        }

        private void UnSubscribe()
        {
            _screenTouchReporter.OnScreenTouched -= ScreenTouchHandler;
            _level.OnWin -= MoveToDoors;
        }
    }
}