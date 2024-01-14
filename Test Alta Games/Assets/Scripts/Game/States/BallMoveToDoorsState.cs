using System.Collections.Generic;
using DG.Tweening;
using Game.States.Interfaces;
using UnityEngine;

namespace Game.States
{
    public class BallMoveToDoorsState : IBallState
    {
        private const int JumpsCount = 12;
        private const float JumpHeight = 1.5f;
        private const float MoveToDoorsDuration = 6f;

        private readonly Ball _ball;

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
            float distanceBetweenPoints = CalculateDistanceToDoors
                (out List<Vector3> movementPoints, out float currentDistance);

            for (int i = 0; i < JumpsCount; i++)
            {
                Vector3 pointToMove;

                if (i % 2 == 0)
                {
                    pointToMove = _ball.transform.position + (_ball.transform
                        .right * currentDistance) + new Vector3(0, JumpHeight, 0);
                }
                else
                {
                    pointToMove = _ball.transform.position + (_ball.transform.right * currentDistance);
                }

                movementPoints.Add(pointToMove);

                currentDistance += distanceBetweenPoints;
            }
            
            _ball.transform.DOPath(movementPoints.ToArray(), MoveToDoorsDuration)
                .SetEase(_ball.JumpEasing);
        }

        private float CalculateDistanceToDoors(out List<Vector3> movementPoints, out float currentDistance)
        {
            float distance = Vector3.Distance(_ball.transform.position, _ball.TargetPoint.position);

            float distanceBetweenPoints = distance / JumpsCount;

            movementPoints = new();

            currentDistance = distanceBetweenPoints;
            
            return distanceBetweenPoints;
        }
    }
}