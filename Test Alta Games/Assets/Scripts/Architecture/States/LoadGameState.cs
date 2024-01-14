using System.Collections;
using Architecture.Services.Interfaces;
using Architecture.States.Interfaces;
using Audio;
using Data;
using Game;
using UI.Game;
using UnityEngine;

namespace Architecture.States
{
    public class LoadGameState : IState
    {
        private const string GameScene = "Game";
        
        private const float ScreenTouchReporterUnlockDelay = 1f;
        
        private readonly ISceneLoader _sceneLoader;
        private readonly IAudioService _audioService;
        private readonly IAssetProvider _assetProvider;
        private readonly IBaseFactory _baseFactory;
        private readonly GameSettings _gameSettings;
        private readonly ICoroutineRunner _coroutineRunner;

        public LoadGameState(ISceneLoader sceneLoader, IAudioService audioService, 
            IAssetProvider assetProvider, IBaseFactory baseFactory, 
            GameSettings gameSettings, ICoroutineRunner coroutineRunner)
        {
            _sceneLoader = sceneLoader;
            _audioService = audioService;
            _assetProvider = assetProvider;
            _baseFactory = baseFactory;
            _gameSettings = gameSettings;
            _coroutineRunner = coroutineRunner;
        }
        
        public void Exit()
        {
            _audioService.StopMusic();
            
            _assetProvider.CleanUp();
        }

        public void Enter()
        {
            _sceneLoader.Load(GameScene, Initialize);
        }

        private async void Initialize()
        {
            Transform parent = (await _baseFactory.CreateAddressableWithObject
                (_gameSettings.BaseParent, Vector3.zero, Quaternion.identity, null)).transform;
            
            Level level = (await _baseFactory.CreateAddressableWithContainer
                (_gameSettings.Level, Vector3.zero, Quaternion.identity, parent)).GetComponent<Level>();
            
            Ball ball = (await _baseFactory.CreateAddressableWithContainer
                (_gameSettings.Ball, level.BallSpawnPosition.position, Quaternion.identity, parent)).GetComponent<Ball>();
            
            Camera camera = (await _baseFactory.CreateAddressableWithContainer
            (_gameSettings.BaseCamera, level.CameraSpawnPosition.position,
                level.CameraSpawnPosition.rotation, parent)).GetComponent<Camera>();
            
            GameView gameView = (await _baseFactory.CreateAddressableWithContainer
                (_gameSettings.GameView, Vector3.zero, Quaternion.identity, parent)).GetComponent<GameView>();
            gameView.GetComponent<Canvas>().worldCamera = camera;
            
            ball.Initialize(gameView.ScreenTouchReporter, level);
            
            _audioService.PlayMusic(MusicType.Game);

            _coroutineRunner.StartCoroutine(UnlockScreenTouchReporter(gameView.ScreenTouchReporter));
        }

        private IEnumerator UnlockScreenTouchReporter(ScreenTouchReporter screenTouchReporter)
        {
            yield return new WaitForSeconds(ScreenTouchReporterUnlockDelay);
            
            screenTouchReporter.SetLockState(false);
        }
    }
}