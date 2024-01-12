using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Data
{
    [CreateAssetMenu(fileName = "Game Settings", menuName = "Create Settings Holder/Game Settings")]
    public class GameSettings : ScriptableObject
    {
        [Header("Prefabs")]
        
        public AssetReferenceGameObject BaseParent;
        public AssetReferenceGameObject BaseCamera;
        public AssetReferenceGameObject MainMenu;
    }
}