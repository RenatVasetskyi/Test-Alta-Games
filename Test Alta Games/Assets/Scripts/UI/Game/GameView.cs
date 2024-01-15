using Architecture.Services.Interfaces;
using Audio;
using Game.Interfaces;
using UnityEngine;
using Zenject;

namespace UI.Game
{
    public class GameView : MonoBehaviour
    {
        public ScreenTouchReporter ScreenTouchReporter;

        [SerializeField] private GameOverWindow _victoryWindow;
        [SerializeField] private GameOverWindow _loseWindow;

        private IAudioService _audioService;
        
        private IGameOverReporter _gameOverReporter;

        [Inject]
        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }
        
        public void Initialize(IGameOverReporter gameOverReporter)
        {
            _gameOverReporter = gameOverReporter;
            
            Subscribe();
        }

        private void Awake()
        {
            _victoryWindow.gameObject.SetActive(false);
            _loseWindow.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            UnSubscribe();
        }

        private void ShowVictory()
        {
            _victoryWindow.gameObject.SetActive(true);

            _victoryWindow.ShowText();
            
            _audioService.StopMusic();
            
            _audioService.PlaySfx(SfxType.Victory);
        }

        private void ShowLoseWindow()
        {
            _loseWindow.gameObject.SetActive(true);

            _loseWindow.ShowText();
            
            _audioService.StopMusic();
            
            _audioService.PlaySfx(SfxType.Lose);
        }

        private void Subscribe()
        {
            _gameOverReporter.OnWin += ShowVictory;
            _gameOverReporter.OnLose += ShowLoseWindow;
        }
        
        private void UnSubscribe()
        {
            _gameOverReporter.OnWin -= ShowVictory;
            _gameOverReporter.OnLose -= ShowLoseWindow;
        }
    }
}