using JetBrains.Annotations;
using UnityEngine;

namespace Coimbra.Settings
{
    [CreateAssetMenu(menuName = FrameworkUtility.GeneralMenuPath + "Pooling Settings")]
    public sealed class PoolingSettings : ScriptableSettings
    {
        /// <summary>
        /// Default pools to be created when a new Pooling Service is being created.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Default pools to be created when a new Pooling Service is being created.")]
        [PublicAPI]
        public LazyLoadReference<GameObjectPool>[] DefaultPersistentPools { get; set; }
    }
}
