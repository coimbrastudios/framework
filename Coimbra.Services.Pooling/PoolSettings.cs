using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Coimbra.Services.Pooling
{
    /// <summary>
    /// <see cref="ScriptableSettingsType.RuntimeProjectSettings"/> for <see cref="PoolSystem"/>.
    /// </summary>
    [ProjectSettings(CoimbraUtility.ProjectSettingsPath)]
    public sealed class PoolSettings : ScriptableSettings
    {
        [SerializeField]
        [AssetReferenceComponentRestriction(typeof(GameObjectPool))]
        [Tooltip("Default pools to be created when a new Pooling Service is being created.")]
        private AssetReferenceT<GameObject>[] _defaultPersistentPools;

        public PoolSettings()
        {
            _defaultPersistentPools = Array.Empty<AssetReferenceT<GameObject>>();
        }

        /// <param name="defaultPersistentPools">Default pools to be created when a new Pooling Service is being created.</param>
        public PoolSettings(IEnumerable<AssetReferenceT<GameObject>> defaultPersistentPools)
        {
            _defaultPersistentPools = defaultPersistentPools.ToArray();
        }

        /// <summary>
        /// Default pools to be created when a new Pooling Service is being created.
        /// </summary>
        public IReadOnlyList<AssetReferenceT<GameObject>> DefaultPersistentPools => _defaultPersistentPools;
    }
}
