﻿using System.Collections;
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
        
        [Inject]
        public void Construct(GameSettings gameSettings, IBaseFactory baseFactory)
        {
            _gameSettings = gameSettings;
            _baseFactory = baseFactory;
        }
        
        public void Initialize(IScreenTouchReporter screenTouchReporter, PathLine pathLine)
        {
            _screenTouchReporter = screenTouchReporter;
            _pathLine = pathLine;

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

            newBall.transform.localScale = Vector3.zero;
            
            StartCoroutine(ScaleBall(newBall));
        }

        private IEnumerator ScaleBall(DestroyableBall newBall)
        {
            while (_isScreenTouched)
            {
                float ballCurrentScalePercent = ReduceScale(ScalePercentStep);

                if (ballCurrentScalePercent < CriticalScalePercent)
                {
                    Debug.Log("Lose");
                    
                    Destroy(gameObject);
                }
                else
                {
                    newBall.AddScale(ScalePercentStep);
                    _pathLine.ReduceScale(ScalePercentStep);
                }
                
                yield return new WaitForSeconds(TimeStep);
            }
        }

        private float ReduceScale(float percentStep)
        {
            float currentPercent = transform.localScale.y * MaxScalePercent / _startScale;
            
            float scaleToReduce = (_startScale / MaxScalePercent) * percentStep;

            if (currentPercent > percentStep)
                transform.localScale -= new Vector3(scaleToReduce, scaleToReduce, scaleToReduce);

            return currentPercent;
        }

        private void MoveNewBall()
        {
            
        }

        private void ScreenTouchHandler(bool isTouched)
        {
            _isScreenTouched = isTouched;
            
            if (isTouched)
                CreateNewBall();
            else
                MoveNewBall();
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