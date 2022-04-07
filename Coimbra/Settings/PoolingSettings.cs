using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Coimbra.Settings
{
    [CreateAssetMenu(menuName = FrameworkUtility.GeneralMenuPath + "Pooling Settings")]
    public sealed class PoolingSettings : ScriptableSettings
    {
        [SerializeField]
        [AssetReferenceComponentRestriction(typeof(GameObjectPool))]
        [Tooltip("Default pools to be created when a new Pooling Service is being created.")]
        private AssetReferenceT<GameObject>[] _defaultPersistentPools;

        public PoolingSettings(AssetReferenceT<GameObject>[] defaultPersistentPools)
        {
            _defaultPersistentPools = defaultPersistentPools;
        }

        /// <summary>
        /// Default pools to be created when a new Pooling Service is being created.
        /// </summary>
        public IReadOnlyList<AssetReferenceT<GameObject>> DefaultPersistentPools => _defaultPersistentPools;
    }
}
