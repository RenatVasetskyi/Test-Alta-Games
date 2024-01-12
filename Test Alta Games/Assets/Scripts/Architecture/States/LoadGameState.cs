using Architecture.Services.Interfaces;
using Architecture.States.Interfaces;
using Audio;

namespace Architecture.States
{
    public class LoadGameState : IState
    {
        private const string GameScene = "Game";
        
        private readonly ISceneLoader _sceneLoader;
        private readonly IAudioService _audioService;
        private readonly IAssetProvider _assetProvider;

        public LoadGameState(ISceneLoader sceneLoader, IAudioService audioService, 
            IAssetProvider assetProvider)
        {
            _sceneLoader = sceneLoader;
            _audioService = audioService;
            _assetProvider = assetProvider;
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

        private void Initialize()
        {
            _audioService.PlayMusic(MusicType.Game);
        }
    }
}