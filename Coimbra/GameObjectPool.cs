using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Scripting;
using Debug = UnityEngine.Debug;

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
            Unloaded = 0,

            /// <summary>
            /// Pool is preloading its instances.
            /// </summary>
            Loading = 1,

            /// <summary>
            /// Pool is completely loaded and ready to be used.
            /// </summary>
            Loaded = 2,
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

        private int? _loadFrame;

        private Actor _prefabActor;

        private GameObjectPool() { }

        /// <summary>
        /// Gets the amount of instances currently available.
        /// </summary>
        public int AvailableInstancesCount => _availableInstances?.Count ?? 0;

        /// <summary>
        /// Gets the amount of instances that still need to preload.
        /// </summary>
        [field: SerializeField]
        [field: Disable]
        [field: Tooltip("The amount of instances that still need to preload.")]
        public int PreloadingInstancesCount { get; private set; }

        /// <summary>
        /// Gets the current pool state.
        /// </summary>
        public State CurrentState => _currentState;

        /// <summary>
        /// Gets or sets a value indicating whether  instantiate will be used when there is no available instance.
        /// </summary>
        public bool CanInstantiateOnSpawn
        {
            [DebuggerStepThrough]
            get => _canInstantiateOnSpawn;
            [DebuggerStepThrough]
            set => _canInstantiateOnSpawn = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether new instances will receive a more descriptive name (Editor Only).
        /// </summary>
        public bool ChangeNameOnInstantiate
        {
            [DebuggerStepThrough]
            get => _changeNameOnInstantiate;
            [DebuggerStepThrough]
            set => _changeNameOnInstantiate = value;
        }

        /// <summary>
        /// Gets or sets the amount of instances to preload each time we are below the minimum amount of desired available instances. If 0, then it will never expand.
        /// </summary>
        public int ExpandStep
        {
            [DebuggerStepThrough]
            get => _expandStep;
            set => _expandStep = Mathf.Max(value, 0);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the parent will not change automatically when despawned. Changing to false affects performance.
        /// </summary>
        public bool KeepParentOnDespawn
        {
            [DebuggerStepThrough]
            get => _keepParentOnDespawn;
            [DebuggerStepThrough]
            set => _keepParentOnDespawn = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this pool will automatically load when initializing.
        /// </summary>
        public bool LoadOnInitialize
        {
            [DebuggerStepThrough]
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
        /// Gets or sets the amount of instance to destroy when we are above the maximum amount of desired available instances by this same amount. If 0, then it will never shrink.
        /// </summary>
        public int ShrinkStep
        {
            [DebuggerStepThrough]
            get => _shrinkStep;
            set => _shrinkStep = Mathf.Max(value, 0);
        }

        /// <summary>
        /// Gets or sets the range of instances desired to be available.
        /// </summary>
        public IntRange DesiredAvailableInstancesRange
        {
            [DebuggerStepThrough]
            get => _desiredAvailableInstancesRange;
            set => _desiredAvailableInstancesRange = new IntRange(Mathf.Max(value.Min, 0), Mathf.Max(value.Max, 0));
        }

        /// <summary>
        /// Gets or sets the prefab that this pool is using.
        /// </summary>
        public AssetReferenceT<GameObject> PrefabReference
        {
            [DebuggerStepThrough]
            get => _prefabReference;
            set
            {
                if (_currentState != State.Unloaded)
                {
                    Debug.LogError($"Can't change the prefab of a {nameof(GameObjectPool)} currently in use!", GameObject);
                }
                else
                {
                    _prefabReference = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the transform to be used as the container.
        /// </summary>
        public Transform ContainerTransform
        {
            [DebuggerStepThrough]
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
                    instance.Transform.SetParent(_containerTransform, false);
                }
            }
        }

        /// <summary>
        /// Loads this pool, instancing the amount of preloaded instances in the process.
        /// </summary>
        public async UniTask LoadAsync(CancellationToken cancellationToken = default)
        {
            if (IsDestroyed)
            {
                Debug.LogWarning($"Attempting to load the already destroyed pool {GameObject}! This will have no effect.", GameObject);

                return;
            }

            Initialize();

            if (_currentState != State.Unloaded)
            {
                Debug.LogWarning($"Pool {GameObject} is {_currentState} already!", GameObject);

                return;
            }

            if (_prefabReference == null)
            {
                Debug.LogError($"{GameObject} requires a non-null prefab to load!", GameObject);

                return;
            }

            GameObject.SetActive(false);
            ChangeCurrentState(State.Loading);

            try
            {
                cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, DestroyCancellationToken).Token;

                GameObject prefab = await _prefabReference.LoadAssetAsync().Task.AsUniTask().AttachExternalCancellation(cancellationToken);

                if (!_prefabReference.IsValid())
                {
                    return;
                }

                _loadFrame = Time.frameCount;
                _prefabActor = prefab.AsActor();
                _availableInstances = new List<Actor>(_desiredAvailableInstancesRange.Max + _shrinkStep);

                await PreloadAsync(_desiredAvailableInstancesRange.Max, cancellationToken);

                ChangeCurrentState(State.Loaded);
            }
            catch (Exception e)
            {
                if (e.IsOperationCanceledException())
                {
                    Unload(false);
                }
                else
                {
                    Debug.LogException(e, GameObject);
                }
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
                Debug.LogWarning($"{GameObject} is useless while unloaded!", GameObject);

                return null;
            }

            Actor instance;

            do
            {
                if (_availableInstances.Count == 0)
                {
                    return _canInstantiateOnSpawn ? CreateInstance(parent, spawnInWorldSpace) : null;
                }

                instance = _availableInstances.Pop();
            }
            while (!instance.IsValid());

            if (PreloadingInstancesCount > 0)
            {
                PreloadingInstancesCount++;
            }
            else if (_expandStep > 0 && _availableInstances.Count < _desiredAvailableInstancesRange.Min)
            {
                PreloadAsync(_expandStep).Forget();
            }

            instance.Transform.SetParent(parent, spawnInWorldSpace);
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
                Debug.LogWarning($"{GameObject} is useless while unloaded!", GameObject);

                return null;
            }

            Actor instance;

            do
            {
                if (_availableInstances.Count == 0)
                {
                    return _canInstantiateOnSpawn ? CreateInstance(position, rotation, parent) : null;
                }

                instance = _availableInstances.Pop();
            }
            while (!instance.IsValid());

            if (PreloadingInstancesCount > 0)
            {
                PreloadingInstancesCount++;
            }
            else if (_expandStep > 0 && _availableInstances.Count < _desiredAvailableInstancesRange.Min)
            {
                PreloadAsync(_expandStep).Forget();
            }

            instance.Transform.parent = parent;
            instance.Transform.SetPositionAndRotation(position, rotation);
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
        public bool Unload(bool logWarning = true)
        {
            if (_currentState == State.Unloaded)
            {
                if (logWarning)
                {
                    Debug.LogWarning($"Pool {GameObject} is unloaded already!", GameObject);
                }

                return false;
            }

            _loadFrame = null;
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
#pragma warning disable UNT0008
                    _availableInstances.Pop().GetValid()?.Destroy();
#pragma warning restore UNT0008
                }

                _availableInstances = null;
            }

            ChangeCurrentState(State.Unloaded);

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Despawn(Actor instance)
        {
            if (!_keepParentOnDespawn)
            {
                instance.Transform.SetParent(_containerTransform, false);
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

        /// <inheritdoc/>
        protected override void OnValidate()
        {
            base.OnValidate();

            DesiredAvailableInstancesRange = _desiredAvailableInstancesRange;
        }

        /// <inheritdoc/>
        protected override void OnInitialize()
        {
            base.OnInitialize();

            if (_loadOnInitialize)
            {
                LoadAsync().Forget();
            }
        }

        /// <inheritdoc/>
        protected override void OnDestroyed()
        {
            base.OnDestroyed();

            if (_currentState != State.Unloaded)
            {
                Unload();
            }
        }

        /// <inheritdoc/>
        protected override void Reset()
        {
            base.Reset();

            _loadOnInitialize = true;
            _containerTransform = transform;
        }

        private async UniTask PreloadAsync(int count, CancellationToken cancellationToken = default)
        {
            Debug.Assert(_loadFrame.HasValue);

            int savedLoadFrame = _loadFrame.Value;
            PreloadingInstancesCount = count;
            _availableInstances.EnsureCapacity(_availableInstances.Count + count);

            while (PreloadingInstancesCount > 0)
            {
                AsyncOperationHandle<GameObject> operationHandle = _prefabReference.InstantiateAsync(_containerTransform);
                GameObject instance = null;

                try
                {
                    instance = await operationHandle.Task.AsUniTask().AttachExternalCancellation(cancellationToken);
                }
                catch (Exception e) when (!(e is OperationCanceledException))
                {
                    Debug.LogException(e);
                }

                if (instance == null)
                {
                    return;
                }

                if (_loadFrame == null || savedLoadFrame != _loadFrame)
                {
                    Addressables.ReleaseInstance(instance);
                    instance.Destroy();

                    return;
                }

                --PreloadingInstancesCount;
                instance.TryGetComponent(out Actor actorInstance);
                _availableInstances.Add(actorInstance);
                actorInstance.Initialize(this, operationHandle);
                ProcessInstance(actorInstance, false);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ChangeCurrentState(State target)
        {
            State previous = _currentState;
            _currentState = target;
            OnPoolStateChanged?.Invoke(this, previous, _currentState);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Actor CreateInstance(Transform parent, bool instantiateInWorldSpace)
        {
            Actor instance = Instantiate(_prefabActor, parent, instantiateInWorldSpace);
            instance.Initialize(this, default);
            ProcessInstance(instance, true);

            return instance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Actor CreateInstance(Vector3 position, Quaternion rotation, Transform parent)
        {
            Actor instance = Instantiate(_prefabActor, position, rotation, parent);
            instance.Initialize(this, default);
            ProcessInstance(instance, true);

            return instance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ProcessInstance(Actor instance, bool spawn)
        {
#if UNITY_EDITOR
            if (_changeNameOnInstantiate)
            {
                instance.GameObject.name = $"{_prefabReference.editorAsset.name} ({Guid.NewGuid()})";
            }
#endif
            OnInstanceCreated?.Invoke(this, instance);

            if (spawn)
            {
                instance.Spawn();
            }
        }
    }
}
