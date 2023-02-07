#if UNITY_EDITOR
using SceneAsset = UnityEditor.SceneAsset;
#else
using SceneAsset = UnityEngine.Object;
#endif
using System;
using UnityEngine.AddressableAssets;

namespace Coimbra
{
    /// <summary>
    /// Asset reference that only accepts <see cref="SceneAsset"/>.
    /// </summary>
    [Serializable]
    public class AssetReferenceScene : AssetReferenceT<SceneAsset>
    {
        public AssetReferenceScene(string guid)
            : base(guid) { }
    }
}
