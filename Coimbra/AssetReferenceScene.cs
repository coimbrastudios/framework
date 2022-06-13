using System;
using UnityEngine.AddressableAssets;

namespace Coimbra
{
    /// <summary>
    /// Scene only asset reference.
    /// </summary>
    [Serializable]
    public class AssetReferenceScene
#if UNITY_EDITOR
        : AssetReferenceT<UnityEditor.SceneAsset>
#else
        : AssetReferenceT<UnityEngine.Object>
#endif
    {
        public AssetReferenceScene(string guid)
            : base(guid) { }
    }
}
