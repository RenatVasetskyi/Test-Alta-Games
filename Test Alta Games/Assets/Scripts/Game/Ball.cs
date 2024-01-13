using Architecture.Services.Interfaces;
using Data;
using UI.Game.Interfaces;
using UnityEngine;
using Zenject;

namespace Game
{
    public class Ball : MonoBehaviour
    {
        [SerializeField] private SphereCollider _collider;

        private GameSettings _gameSettings;
        private IBaseFactory _baseFactory;
        
        private IScreenTouchReporter _screenTouchReporter;
        private DestroyableBall _destroyableBallPrefab;

        [Inject]
        public void Construct(GameSettings gameSettings, IBaseFactory baseFactory)
        {
            _gameSettings = gameSettings;
            _baseFactory = baseFactory;
        }
        
        public void Initialize(IScreenTouchReporter screenTouchReporter)
        {
            _screenTouchReporter = screenTouchReporter;

            Subscribe();
        }

        private void OnDestroy()
        {
            UnSubscribe();
        }

        private async void CreateNewBall()
        { 
            GameObject newBall = await _baseFactory.CreateAddressableWithContainer
                (_gameSettings.DestroyableBall, transform.position + transform.right *
                    _collider.radius * 2, Quaternion.identity, transform.parent);
        }

        private void MoveNewBall()
        {
        }

        private void ScreenTouchHandler(bool isTouched)
        {
            if (isTouched)
                CreateNewBall();
            else
                MoveNewBall();
        }
        
        private void Subscribe()
        {
            _screenTouchReporter.OnScreenTouched += ScreenTouchHandler;
        }

        private void UnSubscribe()
        {
            _screenTouchReporter.OnScreenTouched -= ScreenTouchHandler;
        }
    }
}