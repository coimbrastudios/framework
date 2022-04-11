using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Stack-based pool for any <see cref="GameObject"/> that makes use of <see cref="Addressables"/> system.
    /// </summary>
    [PublicAPI]
    [Preserve]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "GameObject Pool")]
    public sealed class GameObjectPool : Actor
    {
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
        /// Delegate for listening when an instance is created.
        /// </summary>
        public delegate void CreateInstanceHandler(GameObjectPool pool, Actor instance);

        /// <summary>
        /// Delegate for listening state changes of a <see cref="GameObjectPool"/>.
        /// </summary>
        public delegate void PoolStateChangeHandler(GameObjectPool pool, State previous, State current);

        /// <summary>
        /// Invoked when a new instance is created.
        /// </summary>
        public event CreateInstanceHandler OnInstanceCreated;

        /// <summary>
        /// Invoked when this pool <see cref="CurrentState"/> changes.
        /// </summary>
        public event PoolStateChangeHandler OnPoolStateChanged;

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
        [Tooltip("The transform to be used as the container.")]
        private Transform _containerTransform;

        [SerializeField]
        [DisableOnPlayMode]
        [Tooltip("If true, this pool will automatically load when initializing.")]
        private bool _loadOnInitialize;

        [SerializeField]
        [Tooltip("If true, instantiate will be used when there is no available instance.")]
        private bool _canInstantiateOnSpawn = true;

        [SerializeField]
        [Tooltip("If true, new instances will receive a more descriptive name. (Editor Only)")]
        private bool _changeNameOnInstantiate = true;

        [SerializeField]
        [Tooltip("Should all preloaded instances start deactivated?")]
        private bool _deactivatePreloadedInstances = true;

        [SerializeField]
        [Tooltip("If true, parent will not change automatically when despawned.")]
        private bool _keepParentOnDespawn;

        [SerializeField]
        [DisableOnPlayMode]
        [Tooltip("The range of instances desired to be available.")]
        private IntRange _desiredAvailableInstancesRange = 1;

        [SerializeField]
        [DisableOnPlayMode]
        [Min(0)]
        [Tooltip("The amount of instances to preload each time we are below the minimum amount of desired available instances. If 0, then it will never expand.")]
        private int _expandStep;

        [SerializeField]
        [DisableOnPlayMode]
        [Min(0)]
        [Tooltip("Destroy this amount of instance when we are above the maximum amount of desired available instances by this same amount. If 0, then it will never shrink.")]
        private int _shrinkStep;

        [SerializeField]
        [Disable]
        [Tooltip("The instances currently available.")]
        private List<Actor> _availableInstances;

        private int _loadFrame;

        private Actor _prefabActor;

        private GameObjectPool() { }

        /// <summary>
        /// The amount of instances currently available.
        /// </summary>
        public int AvailableInstancesCount => _availableInstances?.Count ?? 0;

        /// <summary>
        /// The amount of instances that still need to preload.
        /// </summary>
        [field: SerializeField]
        [field: Disable]
        [field: Tooltip("The amount of instances that still need to preload.")]
        public int PreloadingInstancesCount { get; private set; }

        /// <summary>
        /// The current pool state.
        /// </summary>
        public State CurrentState => _currentState;

        /// <summary>
        /// If true, instantiate will be used when there is no available instance.
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
        /// The amount of instances to preload each time we are below the minimum amount of desired available instances. If 0, then it will never expand.
        /// </summary>
        public int ExpandStep
        {
            get => _expandStep;
            set => _expandStep = Mathf.Max(value, 0);
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
        /// If true, this pool will automatically load when initializing.
        /// </summary>
        public bool LoadOnInitialize
        {
            get => _loadOnInitialize;
            set
            {
                _loadOnInitialize = value;

                if (_loadOnInitialize && IsInitialized && _currentState == State.Unloaded)
                {
                    LoadAsync().Forget();
                }
            }
        }

        /// <summary>
        /// Destroy this amount of instance when we are above the maximum amount of desired available instances by this same amount. If 0, then it will never shrink.
        /// </summary>
        public int ShrinkStep
        {
            get => _shrinkStep;
            set => _shrinkStep = Mathf.Max(value, 0);
        }

        /// <summary>
        /// The range of instances desired to be available.
        /// </summary>
        public IntRange DesiredAvailableInstancesRange
        {
            get => _desiredAvailableInstancesRange;
            set => _desiredAvailableInstancesRange = new IntRange(Mathf.Max(value.Min, 0), Mathf.Max(value.Max, 0));
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

                foreach (Actor instance in _availableInstances)
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
            if (IsDestroyed)
            {
                Debug.LogWarning($"Attempting to load the already destroyed pool {CachedGameObject}! This will have no effect.", CachedGameObject);

                return;
            }

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

            CachedGameObject.SetActive(false);
            ChangeCurrentState(State.Loading);

            try
            {
                GameObject prefab = await _prefabReference.LoadAssetAsync().Task;

                if (!_prefabReference.IsValid())
                {
                    return;
                }

                _loadFrame = Time.frameCount;
                _prefabActor = prefab.AsActor();
                _availableInstances = new List<Actor>(_desiredAvailableInstancesRange.Max + _shrinkStep);

                await PreloadAsync(_desiredAvailableInstancesRange.Max);

                ChangeCurrentState(State.Loaded);
            }
            catch (Exception e)
            {
                Debug.LogException(e, gameObject);
            }
        }

        /// <summary>
        /// Spawns the an instance with the specified transform.
        /// </summary>
        /// <param name="parent">The instance parent.</param>
        /// <param name="spawnInWorldSpace">If false, the instance transform will be relative to the specified parent.</param>
        /// <returns>The spawned instance.</returns>
        public Actor Spawn(Transform parent = null, bool spawnInWorldSpace = false)
        {
            if (_currentState == State.Unloaded)
            {
                Debug.LogWarning($"{CachedGameObject} is useless while unloaded!", CachedGameObject);

                return null;
            }

            if (_availableInstances.Count == 0)
            {
                return _canInstantiateOnSpawn ? CreateInstance(parent, spawnInWorldSpace) : null;
            }

            Actor instance = _availableInstances.Pop();

            if (PreloadingInstancesCount > 0)
            {
                PreloadingInstancesCount++;
            }
            else if (_expandStep > 0 && _availableInstances.Count < _desiredAvailableInstancesRange.Min)
            {
                PreloadAsync(_expandStep).Forget();
            }

            instance.CachedTransform.SetParent(parent, spawnInWorldSpace);
            instance.Spawn();

            return instance;
        }

        /// <summary>
        /// Spawns the an instance with the specified transform.
        /// </summary>
        /// <param name="position">The position in world space.</param>
        /// <param name="rotation">The rotation in world space..</param>
        /// <param name="parent">The instance parent.</param>
        /// <returns>The spawned instance.</returns>
        public Actor Spawn(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (_currentState == State.Unloaded)
            {
                Debug.LogWarning($"{CachedGameObject} is useless while unloaded!", CachedGameObject);

                return null;
            }

            if (_availableInstances.Count == 0)
            {
                return _canInstantiateOnSpawn ? CreateInstance(position, rotation, parent) : null;
            }

            Actor instance = _availableInstances.Pop();

            if (PreloadingInstancesCount > 0)
            {
                PreloadingInstancesCount++;
            }
            else if (_expandStep > 0 && _availableInstances.Count < _desiredAvailableInstancesRange.Min)
            {
                PreloadAsync(_expandStep).Forget();
            }

            instance.CachedTransform.parent = parent;
            instance.CachedTransform.SetPositionAndRotation(position, rotation);
            instance.Spawn();

            return instance;
        }

        /// <inheritdoc cref="GameObjectPool.Spawn(Vector3, Quaternion, Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Actor Spawn(Vector3 position, Vector3 rotation, Transform parent = null)
        {
            return Spawn(position, Quaternion.Euler(rotation), parent);
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

            _loadFrame = 0;
            _prefabActor = null;
            _prefabReference.ReleaseAsset();
            PreloadingInstancesCount = 0;

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

            ChangeCurrentState(State.Unloaded);

            return true;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            if (_loadOnInitialize)
            {
                LoadAsync().Forget();
            }
        }

        protected override void OnDestroying()
        {
            base.OnDestroying();

            if (_currentState != State.Unloaded)
            {
                Unload();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Despawn(Actor instance)
        {
            if (!_keepParentOnDespawn)
            {
                instance.CachedTransform.SetParent(_containerTransform, false);
            }

            _availableInstances.Add(instance);

            if (PreloadingInstancesCount > 0)
            {
                PreloadingInstancesCount--;
            }
            else if (_shrinkStep > 0 && _availableInstances.Count >= _desiredAvailableInstancesRange.Max + _shrinkStep)
            {
                for (int i = 0; i < _shrinkStep; i++)
                {
                    _availableInstances.Pop().Destroy();
                }
            }
        }

        private async UniTask PreloadAsync(int count)
        {
            int savedLoadFrame = _loadFrame;
            bool savedActiveState = _prefabActor.CachedGameObject.activeSelf;

            if (_deactivatePreloadedInstances)
            {
                _prefabActor.CachedGameObject.SetActive(false);
            }

            PreloadingInstancesCount = count;
            _availableInstances.EnsureCapacity(_availableInstances.Count + count);

            while (PreloadingInstancesCount > 0)
            {
                _prefabActor.Pool = this;

                AsyncOperationHandle<GameObject> operationHandle = _prefabReference.InstantiateAsync(_containerTransform);
                GameObject instance = await operationHandle.Task;

                if (savedLoadFrame != _loadFrame)
                {
                    Addressables.ReleaseInstance(instance);
                    Destroy(instance);

                    break;
                }

                --PreloadingInstancesCount;
                instance.TryGetComponent(out Actor actorInstance);
                actorInstance.OperationHandle = operationHandle;
                _availableInstances.Add(actorInstance);
                Initialize(actorInstance, false);
            }

            if (_deactivatePreloadedInstances)
            {
                _prefabActor.CachedGameObject.SetActive(savedActiveState);
            }

            _prefabActor.Pool = null;
        }

        private void Reset()
        {
            _loadOnInitialize = true;
            _containerTransform = transform;
        }

        private void OnValidate()
        {
            DesiredAvailableInstancesRange = _desiredAvailableInstancesRange;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ChangeCurrentState(State target)
        {
            State previous = _currentState;
            _currentState = target;
            OnPoolStateChanged?.Invoke(this, previous, _currentState);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Initialize(Actor instance, bool spawn)
        {
#if UNITY_EDITOR
            if (_changeNameOnInstantiate)
            {
                instance.gameObject.name = $"{_prefabReference.editorAsset.name} ({Guid.NewGuid()})";
            }
#endif
            instance.Initialize();
            OnInstanceCreated?.Invoke(this, instance);

            if (spawn)
            {
                instance.Spawn();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Actor CreateInstance(Transform parent, bool instantiateInWorldSpace)
        {
            _prefabActor.Pool = this;

            Actor instance = Instantiate(_prefabActor, parent, instantiateInWorldSpace);
            _prefabActor.Pool = null;
            Initialize(instance, true);

            return instance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Actor CreateInstance(Vector3 position, Quaternion rotation, Transform parent)
        {
            _prefabActor.Pool = this;

            Actor instance = Instantiate(_prefabActor, position, rotation, parent);
            _prefabActor.Pool = null;
            Initialize(instance, true);

            return instance;
        }
    }
}
