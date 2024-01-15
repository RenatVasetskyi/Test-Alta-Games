using DG.Tweening;
using Game.States.Interfaces;
using UnityEngine;

namespace Game.States
{
    public class BallMoveToDoorsState : IBallState
    {
        private const int JumpsCount = 8;
        private const float JumpPower = 2f;
        private const float MoveToDoorsDuration = 6f;
        private const float RotationDuration = 1f;

        private readonly Vector3 _rotationStep = new(0, 0, -360);
        private readonly Ball _ball;

        private bool _isReachedTarget;

        public BallMoveToDoorsState(Ball ball)
        {
            _ball = ball;
        }
        
        public void Enter()
        {
            MoveToDoors();
        }

        public void Exit()
        {
        }
        
        private void MoveToDoors()
        {
            _ball.transform.DOJump(_ball.TargetPoint.position, JumpPower, JumpsCount, MoveToDoorsDuration)
                .SetEase(_ball.JumpEasing).onComplete += () => _isReachedTarget = true;

            Rotate();
        }

        private void Rotate()
        {
            if (_isReachedTarget)
                return;
            
            Vector3 targetRotation = new Vector3(_ball.Renderer.transform.rotation.x,
                _ball.Renderer.transform.rotation.y, _ball.Renderer.transform.rotation.z) + _rotationStep;
            
            _ball.Renderer.transform.DORotate(targetRotation, RotationDuration, RotateMode.FastBeyond360)
                .SetEase(_ball.RotateEasing).onComplete += Rotate;
        }
    }
}