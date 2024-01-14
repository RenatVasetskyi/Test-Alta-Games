using Game.Interfaces;
using UnityEngine;

namespace UI.Game
{
    public class GameView : MonoBehaviour
    {
        public ScreenTouchReporter ScreenTouchReporter;

        [SerializeField] private GameOverWindow _victoryWindow;
        [SerializeField] private GameOverWindow _loseWindow;
        
        private IGameOverReporter _gameOverReporter;

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
        }

        private void ShowLoseWindow()
        {
            _loseWindow.gameObject.SetActive(true);

            _loseWindow.ShowText();
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