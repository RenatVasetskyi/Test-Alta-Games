using System;
using System.Collections;
using System.Threading.Tasks;
using Architecture.Services.Interfaces;
using Architecture.States;
using Architecture.States.Interfaces;
using Data;
using Game.Interfaces;
using UnityEngine;
using Zenject;

namespace Game
{
    public class Level : MonoBehaviour, IGameOverReporter, 
        IGameObjectScaler, IDestroyableBallCreator
    {
        private const int RestartLoseGameDelay = 2;
        private const int RestartWinGameDelay = 8;
        
        public event Action OnWin;
        public event Action OnLose;
        
        public Transform CameraSpawnPosition;
        public Transform BallSpawnPosition;
        public Transform TargetPoint;
        
        [SerializeField] private PathLine _pathLine;

        private GameSettings _gameSettings;
        private IBaseFactory _baseFactory;
        private IStateMachine _stateMachine;

        private DestroyableBall _newBall;

        [Inject]
        public void Construct(GameSettings gameSettings, IBaseFactory baseFactory, 
            IStateMachine stateMachine)
        {
            _gameSettings = gameSettings;
            _baseFactory = baseFactory;
            _stateMachine = stateMachine;
        }
        
        public async Task<DestroyableBall> CreateDestroyableBall(Ball baseBall, float diameter)
        {
            _newBall = (await _baseFactory.CreateAddressableWithContainer
                (_gameSettings.DestroyableBall, baseBall.transform.position + baseBall.transform.right *
                    diameter * 2, Quaternion.identity, baseBall.transform.parent))
                .GetComponent<DestroyableBall>();
            
            _newBall.Initialize(this);
            
            _newBall.transform.localScale = Vector3.zero;

            return _newBall;
        }

        public void ScaleObjects(float scalePercent, Vector3 scale)
        {
            _pathLine.ReduceScale(scalePercent);
            _newBall.AddScale(scale);
        }

        public void CheckIsHasObstaclesOnPath()
        {
            if (_pathLine.CheckIsHasObstaclesOnPath())
            {
                StartCoroutine(RestartGame(RestartWinGameDelay));
                
                OnWin?.Invoke();
            }
        }
        
        public void SendLose()
        {
            StartCoroutine(RestartGame(RestartLoseGameDelay));
            
            OnLose?.Invoke();
        }

        private IEnumerator RestartGame(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            _stateMachine.Enter<LoadGameState>();
        }
    }
}