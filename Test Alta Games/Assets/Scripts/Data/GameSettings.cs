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
        public AssetReferenceGameObject Level;
        public AssetReferenceGameObject Ball;
        public AssetReferenceGameObject GameView;
        public AssetReferenceGameObject DestroyableBall;
        public AssetReferenceGameObject UICamera;
        public AssetReferenceGameObject LoadingCurtain;
        public AssetReferenceGameObject SfxAudioSource;
        public AssetReferenceGameObject MusicAudioSource;
    }
}