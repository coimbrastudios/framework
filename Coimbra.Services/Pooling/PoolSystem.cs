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
    public sealed class PoolSystem : ServiceActorBase<PoolSystem, IPoolService>, IPoolService
    {
        private readonly List<GameObjectPool> _loadingList = new();

        private readonly HashSet<object> _prefabsSet = new();

        private readonly HashSet<GameObjectPool> _poolsSet = new();

        private readonly Dictionary<GameObjectID, GameObjectPool> _poolFromPrefab = new();

        private PoolSystem() { }

        /// <inheritdoc/>
        public int LoadingPoolCount => _loadingList.Count;

        /// <summary>
        /// Create a new <see cref="IPoolService"/>.
        /// </summary>
        public static IPoolService Create()
        {
            return new GameObject(nameof(PoolSystem)).AddComponent<PoolSystem>();
        }

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

            pool.OnDestroyed += HandlePoolDestroyed;
            pool.OnPoolStateChanged += HandlePoolStateChanged;
            _prefabsSet.Add(pool.PrefabReference.RuntimeKey);
            _loadingList.Add(pool);

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
                _loadingList.Remove(pool);
            }

            _prefabsSet.Remove(pool.PrefabReference.RuntimeKey);
            pool.OnPoolStateChanged -= HandlePoolStateChanged;
            pool.OnDestroyed -= HandlePoolDestroyed;

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
        protected override void OnDestroying()
        {
            base.OnDestroying();
            _poolFromPrefab.Clear();
            _loadingList.Clear();
            _prefabsSet.Clear();

            foreach (GameObjectPool pool in _poolsSet)
            {
                pool.OnDestroyed -= HandlePoolDestroyed;
                pool.OnPoolStateChanged -= HandlePoolStateChanged;
                pool.Unload();
            }

            _poolsSet.Clear();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            DontDestroyOnLoad(CachedGameObject);

            if (!ScriptableSettings.TryGetOrFind(out PoolSettings poolingSettings))
            {
                return;
            }

            IReadOnlyList<AssetReferenceT<GameObject>> defaultPersistentPools = poolingSettings.DefaultPersistentPools;

            for (int i = 0; i < defaultPersistentPools.Count; i++)
            {
                int index = i;

                void handlePersistentPoolInstantiated(AsyncOperationHandle<GameObject> handle)
                {
                    if (IsDestroyed)
                    {
                        Addressables.Release(handle);
                        Destroy(handle.Result);
                    }
                    else if (handle.Result.TryGetComponent(out GameObjectPool pool))
                    {
                        AddPool(pool);
                    }
                    else
                    {
                        Debug.LogError($"Invalid reference, skipping item {index}!", CachedGameObject);
                        Debug.LogError($"Expected a {nameof(GameObjectPool)} on object {handle.Result}.", handle.Result);
                    }
                }

                Addressables.InstantiateAsync(defaultPersistentPools[i], CachedTransform).Completed += handlePersistentPoolInstantiated;
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
            ServiceLocator.Shared.Get<IPoolService>();
        }

        private void HandlePoolDestroyed(Actor pool, DestroyReason reason)
        {
            RemovePool((GameObjectPool)pool, false);
        }

        private void HandlePoolStateChanged(GameObjectPool pool, GameObjectPool.State previous, GameObjectPool.State current)
        {
            switch (current)
            {
                case GameObjectPool.State.Unloaded:
                {
                    _loadingList.Remove(pool);
                    RemovePool(pool, false);

                    break;
                }

                case GameObjectPool.State.Loaded:
                {
                    _loadingList.Remove(pool);
                    _poolFromPrefab.Add(new GameObjectID(pool.PrefabReference.Asset.GetInstanceID()), pool);

                    break;
                }
            }
        }
    }
}
