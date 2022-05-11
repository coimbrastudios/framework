using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Coimbra.Services.Pooling
{
    [ProjectSettings]
    public sealed class PoolSettings : ScriptableSettings
    {
        [SerializeField]
        [AssetReferenceComponentRestriction(typeof(GameObjectPool))]
        [Tooltip("Default pools to be created when a new Pooling Service is being created.")]
        private AssetReferenceT<GameObject>[] _defaultPersistentPools;

        public PoolSettings()
            : this(Array.Empty<AssetReferenceT<GameObject>>()) { }

        public PoolSettings(AssetReferenceT<GameObject>[] defaultPersistentPools)
        {
            _defaultPersistentPools = defaultPersistentPools;
        }

        /// <summary>
        /// Default pools to be created when a new Pooling Service is being created.
        /// </summary>
        public IReadOnlyList<AssetReferenceT<GameObject>> DefaultPersistentPools => _defaultPersistentPools;
    }
}
