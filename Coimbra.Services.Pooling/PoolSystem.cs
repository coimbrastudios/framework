using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Coimbra.Services.Pooling
{
    /// <summary>
    /// Default implementation for <see cref="IPoolService"/>.
    /// </summary>
    [AddComponentMenu("")]
    [PreloadService]
    public sealed class PoolSystem : Actor, IPoolService, ISerializationCallbackReceiver
    {
        private readonly HashSet<object> _prefabsSet = new();

        private readonly HashSet<GameObjectPool> _poolsSet = new();

        private readonly Dictionary<GameObjectID, GameObjectPool> _poolFromPrefab = new();

        [SerializeField]
        [Disable]
        private List<GameObjectPool> _loadedPools = new();

        [SerializeField]
        [Disable]
        private List<GameObjectPool> _loadingPools = new();

        private PoolSystem() { }

        /// <inheritdoc/>
        public int LoadingPoolCount => _loadingPools.Count;

        /// <inheritdoc/>
        public bool AddPool(GameObjectPool pool)
        {
            if (pool == null
             || pool.PrefabReference == null
             || _prefabsSet.Contains(pool.PrefabReference.RuntimeKey)
             || !_poolsSet.Add(pool))
            {
                return false;
            }

            pool.OnDestroying += HandlePoolDestroying;
            pool.OnPoolStateChanged += HandlePoolStateChanged;
            _prefabsSet.Add(pool.PrefabReference.RuntimeKey);
            _loadingPools.Add(pool);

            if (pool.CurrentState == GameObjectPool.State.Unloaded)
            {
                pool.LoadAsync().Forget();
            }

            return true;
        }

        /// <inheritdoc/>
        public bool ContainsPool(GameObjectPool pool)
        {
            return _poolsSet.Contains(pool);
        }

        /// <inheritdoc/>
        public bool ContainsPool(AssetReferenceT<GameObject> prefab)
        {
            return _prefabsSet.Contains(prefab.RuntimeKey);
        }

        /// <inheritdoc/>
        public GameObjectPool[] GetAllPools()
        {
            return _poolsSet.ToArray();
        }

        /// <inheritdoc/>
        public int GetAllPools(List<GameObjectPool> appendResults)
        {
            appendResults.AddRange(_poolsSet);

            return _poolsSet.Count;
        }

        /// <inheritdoc/>
        public bool RemovePool(GameObjectPool pool, bool unload)
        {
            if (pool == null || !_poolsSet.Remove(pool))
            {
                return false;
            }

            if (pool.CurrentState == GameObjectPool.State.Loaded)
            {
                _poolFromPrefab.Remove(new GameObjectID(pool.PrefabReference.Asset.GetInstanceID()));
            }
            else
            {
                _loadingPools.Remove(pool);
            }

            _prefabsSet.Remove(pool.PrefabReference.RuntimeKey);
            pool.OnPoolStateChanged -= HandlePoolStateChanged;
            pool.OnDestroying -= HandlePoolDestroying;

            if (unload && pool.CurrentState != GameObjectPool.State.Unloaded)
            {
                pool.Unload();
            }

            return true;
        }

        /// <inheritdoc/>
        public Actor Spawn([NotNull] GameObject prefab, Transform parent = null, bool spawnInWorldSpace = false)
        {
            if (_poolFromPrefab.TryGetValue(prefab, out GameObjectPool pool))
            {
                return pool.Spawn(parent, spawnInWorldSpace);
            }

            return Instantiate(prefab, parent, spawnInWorldSpace).AsActor();
        }

        /// <inheritdoc/>
        public Actor Spawn([NotNull] GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (_poolFromPrefab.TryGetValue(prefab, out GameObjectPool pool))
            {
                return pool.Spawn(position, rotation, parent);
            }

            return Instantiate(prefab, position, rotation, parent).AsActor();
        }

        /// <inheritdoc/>
        public Actor Spawn([NotNull] GameObject prefab, Vector3 position, Vector3 rotation, Transform parent = null)
        {
            return Spawn(prefab, position, Quaternion.Euler(rotation), parent);
        }

        /// <inheritdoc/>
        public T Spawn<T>([NotNull] T prefab, Transform parent = null, bool spawnInWorldSpace = false)
            where T : Actor
        {
            if (_poolFromPrefab.TryGetValue(prefab.GameObjectID, out GameObjectPool pool))
            {
                return pool.Spawn(parent, spawnInWorldSpace) as T;
            }

            T instance = Instantiate(prefab, parent, spawnInWorldSpace);
            instance.Initialize();

            return instance;
        }

        /// <inheritdoc/>
        public T Spawn<T>([NotNull] T prefab, Vector3 position, Quaternion rotation, Transform parent = null)
            where T : Actor
        {
            if (_poolFromPrefab.TryGetValue(prefab.GameObjectID, out GameObjectPool pool))
            {
                return pool.Spawn(position, rotation, parent) as T;
            }

            T instance = Instantiate(prefab, position, rotation, parent);
            instance.Initialize();

            return instance;
        }

        /// <inheritdoc/>
        public T Spawn<T>([NotNull] T prefab, Vector3 position, Vector3 rotation, Transform parent = null)
            where T : Actor
        {
            return Spawn(prefab, position, Quaternion.Euler(rotation), parent);
        }

        /// <inheritdoc/>
        protected override void OnDestroyed()
        {
            base.OnDestroyed();
            _poolFromPrefab.Clear();
            _loadingPools.Clear();
            _prefabsSet.Clear();

            foreach (GameObjectPool pool in _poolsSet)
            {
                pool.OnDestroying -= HandlePoolDestroying;
                pool.OnPoolStateChanged -= HandlePoolStateChanged;
                pool.Unload();
            }

            _poolsSet.Clear();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            DontDestroyOnLoad(GameObject);

            if (!ScriptableSettings.TryGetOrFind(out PoolSettings settings))
            {
                return;
            }

            IReadOnlyList<AssetReferenceT<GameObject>> defaultPersistentPools = settings.DefaultPersistentPools;

            for (int i = 0; i < defaultPersistentPools.Count; i++)
            {
                int index = i;

                void handlePersistentPoolInstantiated(AsyncOperationHandle<GameObject> handle)
                {
                    if (IsDestroyed)
                    {
                        Addressables.Release(handle);
                        handle.Result.Destroy();
                    }
                    else if (handle.Result.TryGetComponent(out GameObjectPool pool))
                    {
                        AddPool(pool);
                    }
                    else
                    {
                        Debug.LogError($"Invalid reference, skipping item {index}!", GameObject);
                        Debug.LogError($"Expected a {nameof(GameObjectPool)} on object {handle.Result}.", handle.Result);
                    }
                }

                Addressables.InstantiateAsync(defaultPersistentPools[i], Transform).Completed += handlePersistentPoolInstantiated;
            }
        }

        private void HandlePoolDestroying(Actor pool, DestroyReason reason)
        {
            RemovePool((GameObjectPool)pool, false);
        }

        private void HandlePoolStateChanged(GameObjectPool pool, GameObjectPool.State previous, GameObjectPool.State current)
        {
            switch (current)
            {
                case GameObjectPool.State.Unloaded:
                {
                    _loadingPools.Remove(pool);
                    RemovePool(pool, false);

                    break;
                }

                case GameObjectPool.State.Loaded:
                {
                    _loadingPools.Remove(pool);
                    _poolFromPrefab.Add(new GameObjectID(pool.PrefabReference.Asset.GetInstanceID()), pool);

                    break;
                }
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() { }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
#if UNITY_EDITOR
            _loadedPools.Clear();
            _loadedPools.AddRange(_poolsSet);
#endif
        }
    }
}
