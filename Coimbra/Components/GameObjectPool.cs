using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Scripting;

namespace Coimbra
{
    [PublicAPI]
    [Preserve]
    [AddComponentMenu(FrameworkUtility.GeneralMenuPath + "GameObject Pool")]
    public sealed class GameObjectPool : GameObjectBehaviour
    {
        /// <summary>
        /// Results available when trying to despawn an object.
        /// </summary>
        public enum DespawnResult
        {
            /// <summary>
            /// Successfully despawned the object.
            /// </summary>
            Despawned,
            /// <summary>
            /// The object got destroyed.
            /// </summary>
            Destroyed,
            /// <summary>
            /// Aborted the operation because the object does not belong to the pool.
            /// </summary>
            Aborted
        }

        /// <summary>
        /// The <see cref="GameObjectPool"/> state.
        /// </summary>
        public enum State
        {
            /// <summary>
            /// Pool is completely unloaded without any instances on it.
            /// </summary>
            Unloaded,
            /// <summary>
            /// Pool is preloading its instances.
            /// </summary>
            Loading,
            /// <summary>
            /// Pool is completely loaded and ready to be used.
            /// </summary>
            Loaded,
        }

        /// <summary>
        /// Delegate for listening when an object instantiates on a pool.
        /// </summary>
        public delegate void ObjectInstantiateHandler(GameObjectPool pool, GameObjectBehaviour instance);

        /// <summary>
        /// Delegate for listening state changes of a pool.
        /// </summary>
        public delegate void StateChangeHandler(GameObjectPool pool, State previous, State current);

        /// <summary>
        /// Invoked when an object is instantiated.
        /// </summary>
        public event ObjectInstantiateHandler OnObjectInstantiated;

        /// <summary>
        /// Invoked when this pool <see cref="CurrentState"/> changes.
        /// </summary>
        public event StateChangeHandler OnStateChanged;

        [SerializeField]
        [Disable]
        [Tooltip("The current pool state.")]
        private State _currentState;
        [SerializeField]
        [DisableOnPlayMode]
        [Tooltip("The prefab that this pool is using.")]
        private AssetReferenceT<GameObject> _prefabReference;
        [SerializeField]
        [DisableOnPlayMode]
        [Tooltip("If true, this pool will automatically load.")]
        private bool _autoLoad;
        [SerializeField]
        [Tooltip("If true, instantiate will be used when spawn fails.")]
        private bool _canInstantiateOnSpawn = true;
        [SerializeField]
        [Tooltip("If true, new instances will receive a more descriptive name. (Editor Only)")]
        private bool _changeNameOnInstantiate = true;
        [SerializeField]
        [Tooltip("Should all preloaded instances start deactivated?")]
        private bool _deactivatePreloadedInstances = true;
        [SerializeField]
        [Tooltip("If true, parent will not change automatically when despawned. Changing to false affects performance.")]
        private bool _keepParentOnDespawn = true;
        [SerializeField]
        [Min(0)]
        [Tooltip("Amount of instances available from the beginning. This is clamped between 0 and the current Max Capacity.")]
        private int _preloadCount = 1;
        [SerializeField]
        [DisableOnPlayMode]
        [Min(0)]
        [Tooltip("Max amount of instances in the pool. If 0 it is treated as infinity capacity, else it will also clamp the current Preload Count.")]
        private int _maxCapacity = 1;
        [SerializeField]
        [DisableOnPlayMode]
        [Tooltip("The transform to be used as the container.")]
        private Transform _containerTransform;

        private GameObjectBehaviour _prefab;
        private HashSet<GameObjectID> _availableInstancesIds;
        private Stack<GameObjectBehaviour> _availableInstances;

        /// <summary>
        /// The amount of instances currently available.
        /// </summary>
        public int AvailableInstancesCount => _availableInstances?.Count ?? 0;

        /// <summary>
        /// The current pool state.
        /// </summary>
        public State CurrentState => _currentState;

        /// <summary>
        /// If true, this pool will automatically load.
        /// </summary>
        public bool AutoLoad
        {
            get => _autoLoad;
            set
            {
                _autoLoad = value;

                if (_autoLoad && _currentState == State.Unloaded && IsSpawned)
                {
                    LoadAsync().Forget();
                }
            }
        }

        /// <summary>
        /// If true, instantiate will be used when spawn fails.
        /// </summary>
        public bool CanInstantiateOnSpawn
        {
            get => _canInstantiateOnSpawn;
            set => _canInstantiateOnSpawn = value;
        }

        /// <summary>
        /// If true, new instances will receive a more descriptive name. (Editor Only)
        /// </summary>
        public bool ChangeNameOnInstantiate
        {
            get => _changeNameOnInstantiate;
            set => _changeNameOnInstantiate = value;
        }

        /// <summary>
        /// Should all preloaded instances start deactivated?
        /// </summary>
        public bool DeactivatePreloadedInstances
        {
            get => _deactivatePreloadedInstances;
            set => _deactivatePreloadedInstances = value;
        }

        /// <summary>
        /// If true, parent will not change automatically when despawned. Changing to false affects performance.
        /// </summary>
        public bool KeepParentOnDespawn
        {
            get => _keepParentOnDespawn;
            set => _keepParentOnDespawn = value;
        }

        /// <summary>
        /// Amount of instances available from the beginning. This is clamped between 0 and the current <see cref="MaxCapacity"/>.
        /// </summary>
        public int PreloadCount
        {
            get => _preloadCount;
            set => _preloadCount = _maxCapacity == 0 ? Mathf.Max(value, 0) : Mathf.Clamp(value, 0, _maxCapacity);
        }

        /// <summary>
        /// Max amount of instances in the pool. If 0 it is treated as infinity capacity, else it will also clamp the current <see cref="PreloadCount"/>.
        /// </summary>
        public int MaxCapacity
        {
            get => _maxCapacity;
            set
            {
                _maxCapacity = Mathf.Max(value, 0);

                if (_maxCapacity == 0)
                {
                    return;
                }

                _preloadCount = Mathf.Clamp(_preloadCount, 0, _maxCapacity);

                if (_availableInstances == null)
                {
                    return;
                }

                while (_availableInstances.Count > _maxCapacity)
                {
                    GameObjectBehaviour instance = _availableInstances.Pop();

                    if (instance == null)
                    {
                        continue;
                    }

                    _availableInstancesIds.Remove(instance);
                    instance.Destroy();
                }
            }
        }

        /// <summary>
        /// The prefab that this pool is using.
        /// </summary>
        public AssetReferenceT<GameObject> PrefabReference
        {
            get => _prefabReference;
            set
            {
                if (_currentState != State.Unloaded)
                {
                    Debug.LogError($"Can't change the prefab of a {nameof(GameObjectPool)} currently in use!", CachedGameObject);
                }
                else
                {
                    _prefabReference = value;
                }
            }
        }

        /// <summary>
        /// The transform to be used as the container.
        /// </summary>
        public Transform ContainerTransform
        {
            get => _containerTransform;
            set
            {
                if (_containerTransform == value)
                {
                    return;
                }

                _containerTransform = value;

                if (_availableInstances == null || _keepParentOnDespawn)
                {
                    return;
                }

                foreach (GameObjectBehaviour instance in _availableInstances)
                {
                    instance.CachedTransform.SetParent(_containerTransform, false);
                }
            }
        }

        /// <summary>
        /// Loads this pool, instancing the amount of preloaded instances in the process.
        /// </summary>
        public async UniTask LoadAsync()
        {
            if (_currentState != State.Unloaded)
            {
                Debug.LogWarning($"Pool {CachedGameObject} is {_currentState} already!", CachedGameObject);

                return;
            }

            if (_prefabReference == null)
            {
                Debug.LogError($"{CachedGameObject} requires a non-null prefab to load!", CachedGameObject);

                return;
            }

            ChangeCurrentState(State.Loading);

            try
            {
                GameObject prefab = await _prefabReference.LoadAssetAsync().Task;
                _prefab = prefab.GetOrCreateBehaviour();

                GameObjectPool pool = _prefab.Pool;
                _prefab.Pool = this;

                {
                    bool isActive = prefab.activeSelf;

                    if (_deactivatePreloadedInstances)
                    {
                        prefab.SetActive(false);
                    }

                    Queue<Task<GameObject>> tasks = new Queue<Task<GameObject>>(_preloadCount);
                    _availableInstances = new Stack<GameObjectBehaviour>(_preloadCount);
                    _availableInstancesIds = new HashSet<GameObjectID>();

                    for (int i = 0; i < _preloadCount; i++)
                    {
                        tasks.Enqueue(_prefabReference.InstantiateAsync(_containerTransform).Task);
                    }

                    for (int i = 0; i < _preloadCount; i++)
                    {
                        GameObject instance = await tasks.Dequeue();
                        GameObjectBehaviour behaviour = instance.GetOrCreateBehaviour();
                        Instantiate(behaviour);
                        _availableInstances.Push(behaviour);
                        _availableInstancesIds.Add(behaviour);
                    }

                    if (_deactivatePreloadedInstances)
                    {
                        prefab.SetActive(isActive);
                    }
                }

                _prefab.Pool = pool;
                ChangeCurrentState(State.Loaded);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        /// <summary>
        /// Despawns the specified instance.
        /// </summary>
        /// <param name="instance">The instance to despawn.</param>
        /// <returns>The result of the call.</returns>
        public DespawnResult Despawn(GameObject instance)
        {
            return Despawn(instance.GetOrCreateBehaviour());
        }

        /// <inheritdoc cref="GameObjectPool.Despawn(UnityEngine.GameObject)"/>
        public DespawnResult Despawn(GameObjectBehaviour instance)
        {
            if (_currentState == State.Unloaded)
            {
                Debug.LogWarning($"{CachedGameObject} is useless while unloaded!", CachedGameObject);

                return DespawnResult.Aborted;
            }

            GameObjectID id = instance;

            if (_availableInstancesIds.Contains(id))
            {
                return DespawnResult.Aborted;
            }

            if (_maxCapacity == 0 || _availableInstances.Count < _maxCapacity)
            {
                instance.Despawn();

                if (!_keepParentOnDespawn)
                {
                    instance.CachedTransform.SetParent(_containerTransform, false);
                }

                _availableInstances.Push(instance);
                _availableInstancesIds.Add(id);

                return DespawnResult.Despawned;
            }

            instance.Destroy();

            return DespawnResult.Destroyed;
        }

        /// <summary>
        /// Unloads this pool, destroying all the current available instances in the process.
        /// </summary>
        /// <returns>True if was able to unload the pool.</returns>
        public bool Unload()
        {
            if (_currentState == State.Unloaded)
            {
                Debug.LogWarning($"Pool {CachedGameObject} is unloaded already!", CachedGameObject);

                return false;
            }

            _prefabReference.ReleaseAsset();

            if (_availableInstances != null)
            {
                for (int i = 0,
                         count = _availableInstances.Count;
                     i < count;
                     i++)
                {
                    _availableInstances.Pop().GetValid()?.Destroy();
                }

                _availableInstances = null;
            }

            _availableInstancesIds = null;
            ChangeCurrentState(State.Unloaded);

            return true;
        }

        /// <summary>
        /// Spawns the an instance with the specified transform.
        /// </summary>
        /// <param name="parent">The instance parent.</param>
        /// <param name="spawnInWorldSpace">If false, the instance transform will be relative to the specified parent.</param>
        /// <returns>The spawned instance.</returns>
        public GameObjectBehaviour Spawn(Transform parent = null, bool spawnInWorldSpace = false)
        {
            if (_currentState == State.Unloaded)
            {
                Debug.LogWarning($"{CachedGameObject} is useless while unloaded!", CachedGameObject);

                return null;
            }

            while (_availableInstances.Count > 0)
            {
                GameObjectBehaviour instance = _availableInstances.Pop();

                if (instance == null)
                {
                    continue;
                }

                _availableInstancesIds.Remove(instance);
                instance.CachedTransform.SetParent(parent, spawnInWorldSpace);
                instance.Spawn();

                return instance;
            }

            if (_maxCapacity == 0 || _availableInstances.Count < _maxCapacity || _canInstantiateOnSpawn)
            {
                GameObjectBehaviour instance = Instantiate(parent, spawnInWorldSpace);
                instance.Spawn();

                return instance;
            }

            return null;
        }

        /// <summary>
        /// Spawns the an instance with the specified transform.
        /// </summary>
        /// <param name="position">The position in world space.</param>
        /// <param name="rotation">The rotation in world space..</param>
        /// <param name="parent">The instance parent.</param>
        /// <returns>The spawned instance.</returns>
        public GameObjectBehaviour Spawn(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (_currentState == State.Unloaded)
            {
                Debug.LogWarning($"{CachedGameObject} is useless while unloaded!", CachedGameObject);

                return null;
            }

            while (_availableInstances.Count > 0)
            {
                GameObjectBehaviour instance = _availableInstances.Pop();

                if (instance == null)
                {
                    continue;
                }

                _availableInstancesIds.Remove(instance);
                instance.CachedTransform.parent = parent;
                instance.CachedTransform.SetPositionAndRotation(position, rotation);
                instance.Spawn();

                return instance;
            }

            if (_maxCapacity == 0 || _availableInstances.Count < _maxCapacity || _canInstantiateOnSpawn)
            {
                GameObjectBehaviour instance = Instantiate(position, rotation, parent);
                instance.Spawn();

                return instance;
            }

            return null;
        }

        /// <inheritdoc cref="GameObjectPool.Spawn(Vector3, Quaternion, Transform)"/>
        public GameObjectBehaviour Spawn(Vector3 position, Vector3 rotation, Transform parent = null)
        {
            return Spawn(position, Quaternion.Euler(rotation), parent);
        }

        protected override void OnObjectSpawn()
        {
            base.OnObjectSpawn();

            if (_autoLoad)
            {
                LoadAsync().Forget();
            }
        }

        protected override void OnObjectDespawn()
        {
            if (_currentState != State.Unloaded)
            {
                Unload();
            }

            base.OnObjectDespawn();
        }

        private void Reset()
        {
            _autoLoad = true;
            _containerTransform = transform;
        }

        private void OnValidate()
        {
            PreloadCount = _preloadCount;
            MaxCapacity = _maxCapacity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ChangeCurrentState(State target)
        {
            State previous = _currentState;
            _currentState = target;
            OnStateChanged?.Invoke(this, previous, _currentState);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Instantiate(GameObjectBehaviour instance)
        {
#if UNITY_EDITOR
            if (_changeNameOnInstantiate)
            {
                instance.CachedGameObject.name = $"{_prefabReference.editorAsset.name} ({Guid.NewGuid()})";
            }
#endif
            instance.Initialize();
            OnObjectInstantiated?.Invoke(this, instance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private GameObjectBehaviour Instantiate(Transform parent, bool instantiateInWorldSpace)
        {
            GameObjectPool pool = _prefab.Pool;
            _prefab.Pool = this;

            GameObjectBehaviour instance = Instantiate(_prefab, parent, instantiateInWorldSpace);
            _prefab.Pool = pool;

            Instantiate(instance);

            return instance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private GameObjectBehaviour Instantiate(Vector3 position, Quaternion rotation, Transform parent)
        {
            GameObjectPool pool = _prefab.Pool;
            _prefab.Pool = this;

            GameObjectBehaviour instance = Instantiate(_prefab, position, rotation, parent);
            _prefab.Pool = pool;

            Instantiate(instance);

            return instance;
        }
    }
}
