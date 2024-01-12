using Architecture.Services.Interfaces;
using Architecture.States.Interfaces;
using Audio;
using Data;
using UnityEngine;

namespace Architecture.States
{
    public class LoadGameState : IState
    {
        private const string GameScene = "Game";
        
        private readonly ISceneLoader _sceneLoader;
        private readonly IAudioService _audioService;
        private readonly IAssetProvider _assetProvider;
        private readonly IBaseFactory _baseFactory;
        private readonly GameSettings _gameSettings;

        public LoadGameState(ISceneLoader sceneLoader, IAudioService audioService, 
            IAssetProvider assetProvider, IBaseFactory baseFactory, 
            GameSettings gameSettings)
        {
            _sceneLoader = sceneLoader;
            _audioService = audioService;
            _assetProvider = assetProvider;
            _baseFactory = baseFactory;
            _gameSettings = gameSettings;
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
            
            Camera camera = (await _baseFactory.CreateAddressableWithContainer
                (_gameSettings.BaseCamera, Vector3.zero, Quaternion.identity, parent)).GetComponent<Camera>();
            
            _audioService.PlayMusic(MusicType.Game);
        }
    }
}