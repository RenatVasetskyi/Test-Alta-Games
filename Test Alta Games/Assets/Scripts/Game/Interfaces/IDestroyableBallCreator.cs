using System.Threading.Tasks;
using UnityEngine;

namespace Game.Interfaces
{
    public interface IDestroyableBallCreator
    {
        Task<DestroyableBall> CreateDestroyableBall(Transform baseBall, float diameter);
    }
}