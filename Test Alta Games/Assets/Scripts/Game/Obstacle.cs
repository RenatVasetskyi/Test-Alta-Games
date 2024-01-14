using Game.Interfaces;
using UnityEngine;

namespace Game
{
    public class Obstacle : MonoBehaviour, IHideable
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Collider _collider;
        
        public void Hide()
        {
            _meshRenderer.enabled = false;
            _collider.enabled = false;
        }
    }
}