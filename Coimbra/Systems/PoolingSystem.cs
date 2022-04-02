using Coimbra.Settings;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Coimbra
{
    /// <summary>
    /// Default implementation for <see cref="IPoolingService"/>.
    /// </summary>
    [AddComponentMenu("")]
    public sealed class PoolingSystem : ServiceBase<IPoolingService>, IPoolingService
    {
        private readonly List<GameObjectPool> _poolsLoading = new List<GameObjectPool>();
        private readonly HashSet<object> _poolsPrefabs = new HashSet<object>();
        private readonly HashSet<GameObjectPool> _pools = new HashSet<GameObjectPool>();
        private readonly Dictionary<GameObjectID, GameObjectPool> _poolFromPrefab = new Dictionary<GameObjectID, GameObjectPool>();

        /// <inheritdoc/>
        public IReadOnlyList<GameObjectPool> PoolsLoading => _poolsLoading;

        /// <summary>
        /// Create a new <see cref="IPoolingService"/>.
        /// </summary>
        public static IPoolingService Create()
        {
            return new GameObject(nameof(PoolingSystem)).GetOrCreateBehaviour<PoolingSystem>();
        }

        /// <inheritdoc/>
        public bool AddPool(GameObjectPool pool)
        {
            if (pool == null
             || pool.CurrentState != GameObjectPool.State.Unloaded
             || pool.PrefabReference == null
             || _poolsPrefabs.Contains(pool.PrefabReference.RuntimeKey)
             || !_pools.Add(pool))
            {
                return false;
            }

            pool.OnDestroyed += HandlePoolDestroyed;
            pool.OnStateChanged += HandlePoolStateChanged;
            _poolsPrefabs.Add(pool.PrefabReference.RuntimeKey);
            _poolsLoading.Add(pool);
            pool.LoadAsync().Forget();

            return true;
        }

        /// <inheritdoc/>
        public bool ContainsPool(GameObjectPool pool)
        {
            return _pools.Contains(pool);
        }

        /// <inheritdoc/>
        public GameObjectPool.DespawnResult Despawn(GameObject instance)
        {
            if (!instance.TryGetValid(out instance))
            {
                return GameObjectPool.DespawnResult.Aborted;
            }

            if (instance.TryGetComponent(out GameObjectBehaviour behaviour))
            {
                if (behaviour.Pool.TryGetValid(out GameObjectPool pool))
                {
                    return pool.Despawn(instance);
                }

                behaviour.Destroy();
            }
            else if (!Addressables.ReleaseInstance(instance))
            {
                Destroy(instance);
            }

            return GameObjectPool.DespawnResult.Destroyed;
        }

        /// <inheritdoc/>
        public GameObjectPool.DespawnResult Despawn(GameObjectBehaviour instance)
        {
            if (!instance.TryGetValid(out instance))
            {
                return GameObjectPool.DespawnResult.Aborted;
            }

            if (instance.Pool.TryGetValid(out GameObjectPool pool))
            {
                return pool.Despawn(instance);
            }

            instance.Destroy();

            return GameObjectPool.DespawnResult.Destroyed;
        }

        /// <inheritdoc/>
        public GameObjectPool[] GetAllPools()
        {
            return _pools.ToArray();
        }

        /// <inheritdoc/>
        public int GetAllPools(List<GameObjectPool> appendResults)
        {
            appendResults.AddRange(_pools);

            return _pools.Count;
        }

        /// <inheritdoc/>
        public bool RemovePool(GameObjectPool pool)
        {
            if (pool == null || !_pools.Remove(pool))
            {
                return false;
            }

            if (pool.CurrentState == GameObjectPool.State.Loaded)
            {
                _poolFromPrefab.Remove(new GameObjectID(pool.PrefabReference.Asset.GetInstanceID()));
            }

            _poolsPrefabs.Remove(pool.PrefabReference.RuntimeKey);
            pool.OnStateChanged -= HandlePoolStateChanged;
            pool.OnDestroyed -= HandlePoolDestroyed;
            pool.Unload();

            return true;
        }

        /// <inheritdoc/>
        public GameObject Spawn([NotNull] GameObject prefab, Transform parent = null, bool spawnInWorldSpace = false)
        {
            if (_poolFromPrefab.TryGetValue(prefab, out GameObjectPool pool))
            {
                return pool.Spawn(parent, spawnInWorldSpace);
            }

            GameObject instance = Instantiate(prefab, parent, spawnInWorldSpace);

            if (instance.TryGetComponent(out GameObjectBehaviour behaviour))
            {
                behaviour.Initialize();
            }

            return instance;
        }

        /// <inheritdoc/>
        public GameObject Spawn([NotNull] GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (_poolFromPrefab.TryGetValue(prefab, out GameObjectPool pool))
            {
                return pool.Spawn(position, rotation, parent);
            }

            GameObject instance = Instantiate(prefab, position, rotation, parent);

            if (instance.TryGetComponent(out GameObjectBehaviour behaviour))
            {
                behaviour.Initialize();
            }

            return instance;
        }

        /// <inheritdoc/>
        public GameObject Spawn([NotNull] GameObject prefab, Vector3 position, Vector3 rotation, Transform parent = null)
        {
            return Spawn(prefab, position, Quaternion.Euler(rotation), parent);
        }

        /// <inheritdoc/>
        public T Spawn<T>([NotNull] T prefab, Transform parent = null, bool spawnInWorldSpace = false)
            where T : GameObjectBehaviour
        {
            if (_poolFromPrefab.TryGetValue(prefab, out GameObjectPool pool))
            {
                return pool.Spawn(parent, spawnInWorldSpace) as T;
            }

            return Instantiate(prefab, parent, spawnInWorldSpace).GetOrCreateBehaviour<T>();
        }

        /// <inheritdoc/>
        public T Spawn<T>([NotNull] T prefab, Vector3 position, Quaternion rotation, Transform parent = null)
            where T : GameObjectBehaviour
        {
            if (_poolFromPrefab.TryGetValue(prefab, out GameObjectPool pool))
            {
                return pool.Spawn(position, rotation, parent) as T;
            }

            return Instantiate(prefab, position, rotation, parent).GetOrCreateBehaviour<T>();
        }

        /// <inheritdoc/>
        public T Spawn<T>([NotNull] T prefab, Vector3 position, Vector3 rotation, Transform parent = null)
            where T : GameObjectBehaviour
        {
            return Spawn(prefab, position, Quaternion.Euler(rotation), parent);
        }

        /// <inheritdoc/>
        protected override void OnObjectDespawn()
        {
            _poolFromPrefab.Clear();
            _poolsLoading.Clear();
            _poolsPrefabs.Clear();

            foreach (GameObjectPool pool in _pools)
            {
                pool.OnDestroyed -= HandlePoolDestroyed;
                pool.OnStateChanged -= HandlePoolStateChanged;
                pool.Unload();
            }

            _pools.Clear();
            base.OnObjectDespawn();
        }

        protected override void OnObjectSpawn()
        {
            base.OnObjectSpawn();
            DontDestroyOnLoad(CachedGameObject);

            if (!ScriptableSettings.TryGetOrFind(out PoolingSettings poolingSettings))
            {
                return;
            }

            foreach (LazyLoadReference<GameObjectPool> pool in poolingSettings.DefaultPersistentPools)
            {
                if (pool.isSet && !pool.isBroken)
                {
                    AddPool(Instantiate(pool.asset, CachedTransform));
                }
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void HandleSubsystemRegistration()
        {
            ServiceLocator.Shared.SetCreateCallback(Create, false);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void HandleBeforeSceneLoad()
        {
            ServiceLocator.Shared.Get<IPoolingService>();
        }

        private void HandlePoolDestroyed(GameObjectBehaviour sender, DestroyReason reason)
        {
            RemovePool((GameObjectPool)sender);
        }

        private void HandlePoolStateChanged(GameObjectPool pool, GameObjectPool.State previous, GameObjectPool.State current)
        {
            switch (current)
            {
                case GameObjectPool.State.Unloaded:
                {
                    _poolsLoading.Remove(pool);
                    RemovePool(pool);

                    break;
                }

                case GameObjectPool.State.Loaded:
                {
                    _poolsLoading.Remove(pool);
                    _poolFromPrefab.Add(new GameObjectID(pool.PrefabReference.Asset.GetInstanceID()), pool);

                    break;
                }
            }
        }
    }
}
