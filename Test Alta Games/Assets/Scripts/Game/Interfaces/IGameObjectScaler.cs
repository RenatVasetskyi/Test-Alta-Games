using UnityEngine;

namespace Game.Interfaces
{
    public interface IGameObjectScaler
    {
        void ScaleObjects(float scalePercent, Vector3 scale);
    }
}