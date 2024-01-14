using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.Interfaces;
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
        private IGameOverReporter _gameOverReporter;
        private IGameObjectScaler _gameObjectScaler;
        private IDestroyableBallCreator _destroyableBallCreator;
        private Transform _targetPoint;
        
        private bool _isScreenTouched;

        private float _startScale;
        
        public void Initialize(IScreenTouchReporter screenTouchReporter, IGameOverReporter gameOverReporter, 
            IGameObjectScaler gameObjectScaler, IDestroyableBallCreator destroyableBallCreator, 
            Transform targetPoint)
        {
            _screenTouchReporter = screenTouchReporter;
            _gameOverReporter = gameOverReporter;
            _gameObjectScaler = gameObjectScaler;
            _destroyableBallCreator = destroyableBallCreator;
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
            DestroyableBall destroyableBall = await _destroyableBallCreator
                .CreateDestroyableBall(transform, _collider.radius * 2);
            
            StartCoroutine(ScaleBall(destroyableBall));
        }

        private IEnumerator ScaleBall(DestroyableBall newBall)
        {
            while (_isScreenTouched)
            {
                float ballCurrentScalePercent = ReduceScale(ScalePercentStep);

                if (ballCurrentScalePercent < CriticalScalePercent)
                {
                    _gameOverReporter.SendLose();
                    
                    Destroy(gameObject);
                }
                
                yield return new WaitForSeconds(TimeStep);
            }
            
            newBall.Move(_targetPoint.position);
        }

        private float ReduceScale(float percentStep)
        {
            float currentPercent = transform.localScale.y * MaxScalePercent / _startScale;
            
            float scaleToReduce = (_startScale / MaxScalePercent) * percentStep;

            if (currentPercent > percentStep)
            {
                Vector3 scale = new Vector3(scaleToReduce, scaleToReduce, scaleToReduce);
                
                transform.localScale -= scale;
                
                _gameObjectScaler.ScaleObjects(percentStep, scale);
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
            float distance = Vector3.Distance(transform.position, _targetPoint.position);

            float distanceBetweenPoints = distance / JumpsCount;

            movementPoints = new();

            currentDistance = distanceBetweenPoints;
            
            return distanceBetweenPoints;
        }

        private void Subscribe()
        {
            _screenTouchReporter.OnScreenTouched += ScreenTouchHandler;
            _gameOverReporter.OnWin += MoveToDoors;
        }

        private void UnSubscribe()
        {
            _screenTouchReporter.OnScreenTouched -= ScreenTouchHandler;
            _gameOverReporter.OnWin -= MoveToDoors;
        }
    }
}