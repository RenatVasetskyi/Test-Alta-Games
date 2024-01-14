﻿using Architecture.Services.Interfaces;
using DG.Tweening;
using Game.Interfaces;
using Game.States;
using Game.States.Interfaces;
using UI.Game.Interfaces;
using UnityEngine;

namespace Game
{
    public class Ball : MonoBehaviour
    {
        [SerializeField] private SphereCollider _collider;

        [Space]
        
        [SerializeField] private Ease _jumpEasing;
        
        private IScreenTouchReporter _screenTouchReporter;
        private IGameOverReporter _gameOverReporter;
        private IDestroyableBallCreator _destroyableBallCreator;
        
        public Transform TargetPoint { get; private set; }
        public float StartScale { get; private set; }
        public DestroyableBall NewBall { get; private set; }
        public Ease JumpEasing => _jumpEasing;
        public bool IsScreenTouched { get; private set; }

        private BallStateMachine _stateMachine;

        public void Initialize(IScreenTouchReporter screenTouchReporter, IGameOverReporter gameOverReporter, 
            IGameObjectScaler gameObjectScaler, IDestroyableBallCreator destroyableBallCreator, 
            ICoroutineRunner coroutineRunner, Transform targetPoint)
        {
            _screenTouchReporter = screenTouchReporter;
            _gameOverReporter = gameOverReporter;
            _destroyableBallCreator = destroyableBallCreator;
            
            TargetPoint = targetPoint;
            StartScale = transform.localScale.y;

            _stateMachine = new BallStateMachine();
            
            StateFactory stateFactory = new StateFactory(_stateMachine, coroutineRunner,
                gameObjectScaler, gameOverReporter, this);
            
            _stateMachine.Enter<BallIdleState>();
            
            Subscribe();
        }

        private void OnDestroy()
        {
            UnSubscribe();
        }

        private async void CreateNewBall()
        { 
            NewBall = await _destroyableBallCreator
                .CreateDestroyableBall(transform, _collider.radius * 2);
            
            EnterScaleState();
        }

        private void ScaleBalls(bool isTouched)
        {
            IsScreenTouched = isTouched;
            
            if (isTouched)
                CreateNewBall();
            else
                EnterIdleState();
        }
        
        private void Subscribe()
        {
            _screenTouchReporter.OnScreenTouched += ScaleBalls;
            _gameOverReporter.OnWin += EnterMoveToDoorState;
        }

        private void UnSubscribe()
        {
            _screenTouchReporter.OnScreenTouched -= ScaleBalls;
            _gameOverReporter.OnWin -= EnterMoveToDoorState;
        }

        private void EnterScaleState()
        {
            if (_stateMachine.CompareStateWithActive<BallIdleState>()) 
                _stateMachine.Enter<BallScaleState>();
        }
        
        private void EnterMoveToDoorState()
        {
            _stateMachine.Enter<BallMoveToDoorsState>();
        }

        private void EnterIdleState()
        {
            _stateMachine.Enter<BallIdleState>();
        }

        private class StateFactory
        {
            private readonly IBallStateMachine _stateMachine;
            private readonly ICoroutineRunner _coroutineRunner;
            private readonly IGameObjectScaler _gameObjectScaler;
            private readonly IGameOverReporter _gameOverReporter;
            private readonly Ball _ball;

            public StateFactory(IBallStateMachine stateMachine, ICoroutineRunner coroutineRunner, 
                IGameObjectScaler gameObjectScaler, IGameOverReporter gameOverReporter, Ball ball)
            {
                _stateMachine = stateMachine;
                _coroutineRunner = coroutineRunner;
                _gameObjectScaler = gameObjectScaler;
                _gameOverReporter = gameOverReporter;
                _ball = ball;

                CreateStates();
            }

            private void CreateStates()
            {
                CreateBallScaleState();
                CreateBallMoveToDoorState();
                CreateBallIdleState();
            }

            private void CreateBallScaleState()
            {
                _stateMachine.AddState<BallScaleState>(new BallScaleState
                    (_coroutineRunner, _gameOverReporter, _gameObjectScaler, _ball));
            }

            private void CreateBallMoveToDoorState()
            {
                _stateMachine.AddState<BallMoveToDoorsState>
                    (new BallMoveToDoorsState(_ball));
            }

            private void CreateBallIdleState()
            {
                _stateMachine.AddState<BallIdleState>(new BallIdleState());
            }
        }
    }
}