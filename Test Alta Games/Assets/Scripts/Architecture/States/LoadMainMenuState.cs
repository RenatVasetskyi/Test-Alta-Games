using Architecture.Services.Interfaces;
using Architecture.States.Interfaces;
using Audio;
using Data;
using UnityEngine;

namespace Architecture.States
{
    public class LoadMainMenuState : IState
    {
        private const string MainMenuScene = "MainMenu";
        
        private readonly ISceneLoader _sceneLoader;
        private readonly IAudioService _audioService;
        private readonly IAssetProvider _assetProvider;
        private readonly IBaseFactory _baseFactory;
        private readonly GameSettings _gameSettings;

        public LoadMainMenuState(ISceneLoader sceneLoader, IAudioService audioService, 
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
            _sceneLoader.Load(MainMenuScene, Initialize);
        }

        private async void Initialize()
        { 
            Transform parent = (await _baseFactory.CreateAddressableWithObject
                (_gameSettings.BaseParent, Vector3.zero, Quaternion.identity, null)).transform;
            
            Camera camera = (await _baseFactory.CreateAddressableWithContainer
                (_gameSettings.UICamera, Vector3.zero, Quaternion.identity, parent)).GetComponent<Camera>();
            
            Canvas mainMenu = (await _baseFactory.CreateAddressableWithContainer
                (_gameSettings.MainMenu, Vector3.zero, Quaternion.identity, parent)).GetComponent<Canvas>();
            mainMenu.worldCamera = camera;
            
            _audioService.PlayMusic(MusicType.MainMenu);
        }
    }
}