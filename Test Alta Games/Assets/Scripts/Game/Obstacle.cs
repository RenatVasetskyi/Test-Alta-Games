using Game.Interfaces;
using UnityEngine;

namespace Game
{
    public class Obstacle : MonoBehaviour, IDestroyableObstacle
    {
        private const float DestroyDuration = 0.5f;
        
        public void Destroy()
        {
            LeanTween.scale(gameObject, Vector3.zero, DestroyDuration)
                .setOnComplete(() => Destroy(gameObject));
        }
    }
}