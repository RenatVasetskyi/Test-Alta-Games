using System;
using Game.Interfaces;
using UnityEngine;

namespace UI.Game
{
    public class GameView : MonoBehaviour
    {
        public ScreenTouchReporter ScreenTouchReporter;

        private IGameOverReporter _gameOverReporter;
        
        public void Initialize(IGameOverReporter gameOverReporter)
        {
            _gameOverReporter = gameOverReporter;
            
            Subscribe();
        }

        private void OnDestroy()
        {
            UnSubscribe();
        }

        private void ShowVictory()
        {
            
        }

        private void Subscribe()
        {
            _gameOverReporter.OnWin += ShowVictory;
        }
        
        private void UnSubscribe()
        {
            _gameOverReporter.OnWin -= ShowVictory;
        }
    }
}