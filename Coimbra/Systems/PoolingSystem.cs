using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Coimbra
{
    internal sealed class PoolingSystem : IPoolingService
    {
        private readonly List<GameObjectPool> _poolsLoading = new List<GameObjectPool>();
        private readonly HashSet<object> _poolsPrefabs = new HashSet<object>();
        private readonly HashSet<GameObjectPool> _pools = new HashSet<GameObjectPool>();
        private readonly Dictionary<GameObjectID, GameObjectPool> _poolFromPrefab = new Dictionary<GameObjectID, GameObjectPool>();

        /// <inheritdoc/>
        public ServiceLocator OwningLocator { get; set; }

        /// <inheritdoc/>
        public IReadOnlyList<GameObjectPool> PoolsLoading => _poolsLoading;

        /// <inheritdoc/>
        public bool AddPool(GameObjectPool pool, bool isPersistent)
        {
            if (pool == null
             || pool.CurrentState != GameObjectPool.State.Unloaded
             || pool.PrefabReference == null
             || _poolsPrefabs.Contains(pool.PrefabReference.RuntimeKey)
             || !_pools.Add(pool))
            {
                return false;
            }

            pool.OnStateChanged += HandlePoolStateChanged;
            _poolsPrefabs.Add(pool.PrefabReference.RuntimeKey);
            _poolsLoading.Add(pool);
            pool.Load();

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
                Object.Destroy(instance);
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
        public void Dispose()
        {
            // TODO
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
            pool.Unload();

            return true;
        }

        /// <inheritdoc/>
        public bool SetPoolPersistent(GameObjectPool pool, bool isPersistent)
        {
            // TODO

            return false;
        }

        /// <inheritdoc/>
        public GameObject Spawn([NotNull] GameObject prefab, Transform parent = null, bool spawnInWorldSpace = false)
        {
            if (_poolFromPrefab.TryGetValue(prefab, out GameObjectPool pool))
            {
                return pool.Spawn(parent, spawnInWorldSpace);
            }

            GameObject instance = Object.Instantiate(prefab, parent, spawnInWorldSpace);

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

            GameObject instance = Object.Instantiate(prefab, position, rotation, parent);

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

            return Object.Instantiate(prefab, parent, spawnInWorldSpace).GetOrCreateBehaviour() as T;
        }

        /// <inheritdoc/>
        public T Spawn<T>([NotNull] T prefab, Vector3 position, Quaternion rotation, Transform parent = null)
            where T : GameObjectBehaviour
        {
            if (_poolFromPrefab.TryGetValue(prefab, out GameObjectPool pool))
            {
                return pool.Spawn(position, rotation, parent) as T;
            }

            return Object.Instantiate(prefab, position, rotation, parent).GetOrCreateBehaviour() as T;
        }

        /// <inheritdoc/>
        public T Spawn<T>([NotNull] T prefab, Vector3 position, Vector3 rotation, Transform parent = null)
            where T : GameObjectBehaviour
        {
            return Spawn(prefab, position, Quaternion.Euler(rotation), parent);
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
