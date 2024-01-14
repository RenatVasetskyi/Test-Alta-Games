using System.Threading.Tasks;
using UnityEngine;

namespace Game.Interfaces
{
    public interface IDestroyableBallCreator
    {
        Task<DestroyableBall> CreateDestroyableBall(Ball baseBall, float diameter);
    }
}