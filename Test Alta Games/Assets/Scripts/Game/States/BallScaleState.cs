using System.Collections;
using Architecture.Services.Interfaces;
using Game.Interfaces;
using Game.States.Interfaces;
using UnityEngine;

namespace Game.States
{
    public class BallScaleState : IBallState
    {
        private const float ScalePercentStep = 1f;
        private const float TimeStep = 0.05f;

        private const int CriticalScalePercent = 10;
        private const int MaxScalePercent = 100;
        
        private readonly IGameObjectScaler _gameObjectScaler;
        private readonly IBallStateMachine _stateMachine;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IGameOverReporter _gameOverReporter;
        private readonly Ball _ball;
        
        public BallScaleState(IBallStateMachine stateMachine, ICoroutineRunner coroutineRunner,
            IGameOverReporter gameOverReporter, IGameObjectScaler gameObjectScaler, Ball ball)
        {
            _stateMachine = stateMachine;
            _coroutineRunner = coroutineRunner;
            _gameOverReporter = gameOverReporter;
            _gameObjectScaler = gameObjectScaler;
            _ball = ball;
        }
        
        public void Enter()
        {
            _coroutineRunner.StartCoroutine(ScaleBall());
        }

        public void Exit()
        {
        }
        
        private IEnumerator ScaleBall()
        {
            while (_ball.IsScreenTouched)
            {
                float ballCurrentScalePercent = ReduceScale(ScalePercentStep);

                if (ballCurrentScalePercent < CriticalScalePercent)
                {
                    _stateMachine.Enter<BallLoseState>();
                    
                    _gameOverReporter.SendLose();
                    
                    yield break;
                }
                
                yield return new WaitForSeconds(TimeStep);
            }

            _ball.NewBall.Move(_ball.TargetPoint.position);
        }

        private float ReduceScale(float percentStep)
        {
            float currentPercent = _ball.transform.localScale.y * MaxScalePercent / _ball.StartScale;
            
            float scaleToReduce = (_ball.StartScale / MaxScalePercent) * percentStep;

            if (currentPercent > percentStep)
            {
                Vector3 scale = new Vector3(scaleToReduce, scaleToReduce, scaleToReduce);
                
                _ball.transform.localScale -= scale;
                
                _gameObjectScaler.ScaleObjects(percentStep, scale);
            }

            return currentPercent;
        }
    }
}