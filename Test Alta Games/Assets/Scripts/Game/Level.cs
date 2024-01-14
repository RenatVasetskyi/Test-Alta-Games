using System;
using System.Threading.Tasks;
using Architecture.Services.Interfaces;
using Data;
using UnityEngine;
using Zenject;

namespace Game
{
    public class Level : MonoBehaviour
    {
        public event Action OnWin;
        public event Action OnLose;
        
        public Transform CameraSpawnPosition;
        public Transform BallSpawnPosition;
        public Transform TargetPoint;
        
        [SerializeField] private PathLine _pathLine;

        private GameSettings _gameSettings;
        private IBaseFactory _baseFactory;

        private DestroyableBall _newBall;

        [Inject]
        public void Construct(GameSettings gameSettings, IBaseFactory baseFactory)
        {
            _gameSettings = gameSettings;
            _baseFactory = baseFactory;
        }
        
        public async Task<DestroyableBall> CreateDestroyableBall(Transform baseBall, float diameter)
        {
            _newBall = (await _baseFactory.CreateAddressableWithContainer
                (_gameSettings.DestroyableBall, baseBall.position + baseBall.right *
                    diameter * 2, Quaternion.identity, baseBall.parent))
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
                OnWin?.Invoke();
        }

        public void SendLose()
        {
            OnLose?.Invoke();
        }
    }
}