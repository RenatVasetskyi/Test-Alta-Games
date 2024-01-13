using Game.Interfaces;
using UnityEngine;

namespace Game
{
    public class Obstacle : MonoBehaviour, IDestroyableObstacle
    {
        public void Destroy()
        {
            Debug.Log("Destroy");
        }
    }
}