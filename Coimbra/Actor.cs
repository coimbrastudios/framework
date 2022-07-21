using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Coimbra
{
    /// <summary>
    /// Used to represent a <see cref="UnityEngine.GameObject"/>. It is expected to inherit from this class to create the main script of each object.
    /// </summary>
    [PublicAPI]
    [Preserve]
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Actor")]
    public class Actor : MonoBehaviour, IDisposable
    {
        /// <summary>
        /// Defines the reason why the object is being destroyed.
        /// </summary>
        public enum DestroyReason
        {
            /// <summary>
            /// Instigated by an user code explicitly.
            /// </summary>
            ExplicitCall = 0,

            /// <summary>
            /// Instigated by a scene change when the object was not flagged to don't destroy on load.
            /// </summary>
            SceneChange = 1,

            /// <summary>
            /// Being called due the application being shutdown.
            /// </summary>
            ApplicationQuit = 2,
        }

        /// <summary>
        /// Used to represent the <see cref="Actor"/> states.
        /// </summary>
        [Flags]
        public enum StateFlags : byte
        {
            /// <summary>
            /// Default state for when the <see cref="Actor"/> is being created.
            /// </summary>
            None = 0,

            /// <summary>
            /// <see cref="Actor.IsAwaken"/>.
            /// </summary>
            IsAwaken = 1 << 0,

            /// <summary>
            /// <see cref="Actor.IsInitialized"/>.
            /// </summary>
            IsInitialized = 1 << 1,

            /// <summary>
            /// <see cref="Actor.IsPrefab"/>.
            /// </summary>
            IsPrefab = 1 << 2,

            /// <summary>
            /// <see cref="Actor.IsPooled"/>.
            /// </summary>
            IsPooled = 1 << 3,

            /// <summary>
            /// <see cref="Actor.IsSpawned"/>.
            /// </summary>
            IsSpawned = 1 << 4,

            /// <summary>
            /// <see cref="Actor.IsDestroyed"/>.
            /// </summary>
            IsDestroyed = 1 << 5,

            /// <summary>
            /// <see cref="Actor.IsQuitting"/>.
            /// </summary>
            IsQuitting = 1 << 6,

            /// <summary>
            /// <see cref="Actor.IsUnloadingScene"/>.
            /// </summary>
            IsUnloadingScene = 1 << 7,
        }

        /// <summary>
        /// Delegate for handling a <see cref="GameObject"/> active state changes.
        /// </summary>
        public delegate void ActiveStateHandler(Actor sender, bool state);

        /// <summary>
        /// Delegate for handling a <see cref="GameObject"/> destroy.
        /// </summary>
        public delegate void DestroyHandler(Actor sender, DestroyReason reason);

        /// <summary>
        /// Delegate for handling a <see cref="SceneManager.sceneLoaded"/> event after all <see cref="Actor"/> got initialized.
        /// </summary>
        public delegate void SceneInitializedHandler(Scene scene, LoadSceneMode mode);

        /// <summary>
        /// Invoked each time a new <see cref="Scene"/> is loaded and all of its <see cref="Actor"/> got initialized. Use <see cref="OnSceneInitializedOnce"/> if you need to only fire it once.
        /// </summary>
        public static event SceneInitializedHandler OnSceneInitialized;

        /// <summary>
        /// Invoked each time a new <see cref="Scene"/> is loaded and all of its <see cref="Actor"/> got initialized. It resets after each call and is called after <see cref="OnSceneInitialized"/>.
        /// </summary>
        public static event SceneInitializedHandler OnSceneInitializedOnce;

        /// <summary>
        /// Invoked when a <see cref="UnityEngine.GameObject"/> is activated or deactivated in the scene.
        /// </summary>
        public event ActiveStateHandler OnActiveStateChanged;

        /// <summary>
        /// Invoked when a <see cref="UnityEngine.GameObject"/> is being destroyed for any reason.
        /// </summary>
        public event DestroyHandler OnDestroying;

        private static readonly List<Actor> UninitializedActors = new();

        private static readonly Dictionary<GameObjectID, Actor> CachedActors = new();

        private static readonly ProfilerCounterValue<int> NewActorCount = new(ProfilerCategory.Scripts, "New Actors", ProfilerMarkerDataUnit.Count, ProfilerCounterOptions.ResetToZeroOnFlush);

        private static readonly ProfilerCounterValue<int> InitializedActorCount = new(ProfilerCategory.Scripts, "Initialized Actors", ProfilerMarkerDataUnit.Count, ProfilerCounterOptions.FlushOnEndOfFrame);

        private static readonly ProfilerCounterValue<int> UninitializedActorCount = new(ProfilerCategory.Scripts, "Uninitialized Actors", ProfilerMarkerDataUnit.Count, ProfilerCounterOptions.FlushOnEndOfFrame);

        [SerializeField]
        [DisableOnPlayMode]
        [Tooltip(" If true, it will check if Awake was called before Initialize.")]
        private bool _assertAwake;

        [SerializeField]
        [DisableOnPlayMode]
        [FormerlySerializedAsBackingFieldOf("ActivateOnSpawn")]
        [Tooltip("If true, it will activate the object when spawning it.")]
        private bool _activateOnSpawn;

        [SerializeField]
        [DisableOnPlayMode]
        [FormerlySerializedAsBackingFieldOf("DeactivateOnDespawn")]
        [Tooltip("If true, it will deactivate the object when despawning it.")]
        private bool _deactivateOnDespawn;

        private GameObjectID? _gameObjectID;

        private AsyncOperationHandle<GameObject> _operationHandle;

        private CancellationTokenSource _despawnCancellationTokenSource;

        private CancellationTokenSource _destroyCancellationTokenSource;

        private GameObject _gameObject;

        private Transform _transform;

        protected Actor()
        {
            NewActorCount.Value++;
            UninitializedActorCount.Value++;
            UninitializedActors.Add(this);
        }

        /// <summary>
        /// Gets or sets a value indicating whether it will activate the object when spawning it.
        /// </summary>
        public bool ActivateOnSpawn
        {
            [DebuggerStepThrough]
            get => _activateOnSpawn;
            [DebuggerStepThrough]
            set => _activateOnSpawn = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether it will check if <see cref="Awake"/> was called before <see cref="Initialize"/>.
        /// </summary>
        public bool AssertAwake
        {
            [DebuggerStepThrough]
            get => _assertAwake;
            [DebuggerStepThrough]
            set => _assertAwake = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether it will deactivate the object when despawning it.
        /// </summary>
        public bool DeactivateOnDespawn
        {
            [DebuggerStepThrough]
            get => _deactivateOnDespawn;
            [DebuggerStepThrough]
            set => _deactivateOnDespawn = value;
        }

        /// <summary>
        /// Gets the <see cref="CancellationToken"/> for when this <see cref="Actor"/> is about to be despawned.
        /// </summary>
        public CancellationToken DespawnCancellationToken
        {
            get
            {
                if (!IsSpawned)
                {
                    return CancellationToken.None;
                }

                _despawnCancellationTokenSource ??= new CancellationTokenSource();

                return _despawnCancellationTokenSource.Token;
            }
        }

        /// <summary>
        /// Gets the <see cref="CancellationToken"/> for when this <see cref="Actor"/> is about to be destroyed.
        /// </summary>
        public CancellationToken DestroyCancellationToken
        {
            get
            {
                if (IsDestroyed)
                {
                    return CancellationToken.None;
                }

                _destroyCancellationTokenSource ??= new CancellationTokenSource();

                return _destroyCancellationTokenSource.Token;
            }
        }

        /// <summary>
        /// Gets the cached version of <see cref="MonoBehaviour.gameObject"/>.<see cref="Object.GetInstanceID"/>.
        /// </summary>
        public GameObjectID GameObjectID => _gameObjectID ?? (_gameObjectID = GameObject).Value;

        /// <summary>
        /// Gets the cached version of <see cref="MonoBehaviour.gameObject"/> to avoid the C++ interop.
        /// </summary>
        public GameObject GameObject => IsDestroyed || !IsInitialized ? gameObject : _gameObject;

        /// <summary>
        /// Gets the cached version of <see cref="MonoBehaviour.transform"/> to avoid the C++ interop.
        /// </summary>
        public Transform Transform => IsDestroyed || !IsInitialized ? transform : _transform;

        /// <summary>
        /// Gets the current states of this <see cref="Actor"/>.
        /// </summary>
        public StateFlags States { get; private set; }

        /// <summary>
        /// Gets a value indicating whether <see cref="Awake"/> was called already.
        /// </summary>
        public bool IsAwaken => (States & StateFlags.IsAwaken) != 0;

        /// <summary>
        /// Gets a value indicating whether <see cref="Destroy"/> was called at least once in this <see cref="Actor"/> or <see cref="UnityEngine.GameObject"/>.
        /// </summary>
        public bool IsDestroyed => (States & StateFlags.IsDestroyed) != 0;

        /// <summary>
        /// Gets a value indicating whether <see cref="Initialize"/> was called at least once in this <see cref="Actor"/>.
        /// </summary>
        public bool IsInitialized => (States & StateFlags.IsInitialized) != 0;

        /// <summary>
        /// Gets a value indicating whether the object was instantiated through a <see cref="GameObjectPool"/>.
        /// </summary>
        public bool IsPooled => (States & StateFlags.IsPooled) != 0;

        /// <summary>
        /// Gets a value indicating whether this object is a prefab asset.
        /// </summary>
        public bool IsPrefab => (States & StateFlags.IsPrefab) != 0;

        /// <summary>
        /// Gets a value indicating whether the application is quitting.
        /// </summary>
        public bool IsQuitting => (States & StateFlags.IsQuitting) != 0;

        /// <summary>
        /// Gets a value indicating whether the object is currently spawned.
        /// </summary>
        public bool IsSpawned => (States & StateFlags.IsSpawned) != 0;

        /// <summary>
        /// Gets a value indicating whether the scene of this <see cref="Actor"/> is currently unloading.
        /// </summary>
        public bool IsUnloadingScene => (States & StateFlags.IsUnloadingScene) != 0;

        /// <summary>
        /// Gets the pool that owns this instance.
        /// </summary>
        public GameObjectPool Pool { get; private set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator GameObject(Actor actor)
        {
            return actor.GameObject;
        }

        /// <summary>
        /// Returns if the <see cref="Actor"/> representation of specified game object ID is cached.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasCachedActor(GameObjectID gameObjectID, out Actor actor)
        {
            return CachedActors.TryGetValue(gameObjectID, out actor);
        }

        /// <summary>
        /// Initialize all uninitialized actors. This is called for each <see cref="SceneManager.sceneLoaded"/> but can also be used after instantiating any <see cref="Actor"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InitializeActors()
        {
            while (UninitializedActors.Count > 0)
            {
                if (UninitializedActors.Pop().TryGetValid(out Actor actor))
                {
                    actor.Initialize();
                }
            }

            UninitializedActorCount.Value = 0;
        }

        /// <summary>
        /// Despawns the <see cref="UnityEngine.GameObject"/> and return it to its pool. If it doesn't belong to a <see cref="GameObjectPool"/>, it will <see cref="Destroy"/> the object instead.
        /// </summary>
        public void Despawn()
        {
            Despawn(true);
        }

        /// <summary>
        /// Destroys the <see cref="UnityEngine.GameObject"/> that this actor represents.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Destroy()
        {
            Destroy(true);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Destroy(true);
        }

        /// <summary>
        /// Initializes this actor. It will also spawn it if not <see cref="IsPooled"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Initialize()
        {
            Initialize(null, default);
        }

        internal static IReadOnlyCollection<Actor> GetCachedActors()
        {
            return CachedActors.Values;
        }

        internal void Initialize(GameObjectPool pool, AsyncOperationHandle<GameObject> operationHandle)
        {
            const string message = nameof(Actor) + "." + nameof(Initialize) + " was called before the " + nameof(Awake) + " callback but " + nameof(AssertAwake) + " is set to true!";
            Debug.Assert(IsAwaken || !AssertAwake, message, this);

            if (IsDestroyed || IsInitialized)
            {
                return;
            }

            _operationHandle = operationHandle;
            _gameObject = gameObject;
            _transform = transform;
            Pool = pool;
            States |= StateFlags.IsInitialized;

            // should be initialized already to work correctly
            _gameObjectID = GameObject;

            if (GameObject.scene.name == null)
            {
                States |= StateFlags.IsPrefab;
            }

            // should be the last call
            UninitializedActorCount.Value--;
            InitializedActorCount.Value++;
            CachedActors.Add(GameObjectID, this);

            if (IsPrefab)
            {
                OnInitializePrefab();

                return;
            }

            if (Pool != null)
            {
                States |= StateFlags.IsPooled;
            }

            InitializeComponentsAndSelf();

            if (!IsPooled)
            {
                Spawn();
            }
        }

        internal void OnUnloadScene(Scene scene)
        {
            if (GameObject.scene != scene)
            {
                if (Pool.GameObject.scene != scene)
                {
                    return;
                }

                States &= ~StateFlags.IsPooled;
                Pool = null;

                return;
            }

            States |= StateFlags.IsUnloadingScene;

            if (!IsPooled)
            {
                return;
            }

            Despawn();

            if (GameObject.scene != scene)
            {
                States &= ~StateFlags.IsUnloadingScene;

                return;
            }

            if (IsDestroyed || !Pool.KeepParentOnDespawn || Pool.GameObject.scene == scene)
            {
                return;
            }

            States &= ~StateFlags.IsUnloadingScene;
            Transform.SetParent(Pool.Transform, false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Spawn()
        {
            if (IsDestroyed || IsSpawned)
            {
                return;
            }

            States |= StateFlags.IsSpawned;
            OnSpawn();
        }

        /// <summary>
        /// Called each time this object is despawned. By default, it deactivates the object.
        /// </summary>
        protected virtual void OnDespawn()
        {
            if (DeactivateOnDespawn)
            {
                GameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Use this for one-time un-initializations instead of OnDestroy callback. This method is called even if the object starts inactive.
        /// </summary>
        protected virtual void OnDestroyed() { }

        /// <summary>
        /// Use this for one-time initializations instead of Awake callback. This method is called even if the object starts inactive.
        /// </summary>
        protected virtual void OnInitialize() { }

        /// <summary>
        /// Use this for one-time initializations on prefabs.
        /// </summary>
        protected virtual void OnInitializePrefab() { }

        /// <summary>
        /// Called each time this object is spawned. By default, it activates the object.
        /// </summary>
        protected virtual void OnSpawn()
        {
            if (ActivateOnSpawn)
            {
                GameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Unity callback.
        /// </summary>
        protected virtual void OnValidate()
        {
            if (CoimbraUtility.IsPlayMode && !CoimbraUtility.IsFirstFrame)
            {
                Initialize();
            }
        }

        /// <summary>
        /// Unity callback.
        /// </summary>
        protected virtual void Reset()
        {
#if UNITY_EDITOR
            while (UnityEditorInternal.ComponentUtility.MoveComponentUp(this))
            {
                // just moving the component to the top
            }
#endif
        }

        /// <summary>
        /// Non-virtual by design, use <see cref="OnInitialize"/> instead.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void Awake()
        {
            States |= StateFlags.IsAwaken;
        }

#if UNITY_ASSERTIONS
        /// <summary>
        /// Non-virtual by design, use 'Coimbra.Listeners.StartListener' instead or another alternative.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void Start()
        {
            const string message = nameof(Actor) + "." + nameof(Initialize) + " needs to be called before the " + nameof(Start) + " callback!";
            Debug.Assert(IsInitialized, message, this);
        }

#endif

        /// <summary>
        /// Non-virtual by design, use <see cref="OnActiveStateChanged"/> instead.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void OnEnable()
        {
            OnActiveStateChanged?.Invoke(this, true);
        }

        /// <summary>
        /// Non-virtual by design, use <see cref="OnActiveStateChanged"/> instead.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void OnDisable()
        {
            OnActiveStateChanged?.Invoke(this, false);
        }

        /// <summary>
        /// Non-virtual by design, use <see cref="OnDestroyed"/> instead.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void OnDestroy()
        {
            Destroy(false);
        }

        /// <summary>
        /// Non-virtual by design, use <see cref="Application.quitting"/> instead.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void OnApplicationQuit()
        {
            States |= StateFlags.IsQuitting;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void HandleSubsystemRegistration()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private static void HandleSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            InitializeActors();
            OnSceneInitialized?.Invoke(scene, loadSceneMode);

            if (OnSceneInitializedOnce == null)
            {
                return;
            }

            OnSceneInitializedOnce.Invoke(scene, loadSceneMode);
            OnSceneInitializedOnce = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Despawn(bool callPoolDespawnOrDestroy)
        {
            if (!IsSpawned)
            {
                return;
            }

            States |= StateFlags.IsSpawned;
            CancellationTokenSourceUtility.Collect(ref _despawnCancellationTokenSource);
            OnDespawn();

            if (!callPoolDespawnOrDestroy)
            {
                return;
            }

            if (IsPooled && Pool != null && Pool.CurrentState == GameObjectPool.State.Loaded)
            {
                Pool.Despawn(this);
            }
            else
            {
                Destroy(true);
            }
        }

        private void Destroy(bool callObjectDestroy)
        {
            if (IsDestroyed)
            {
                return;
            }

            States |= StateFlags.IsDestroyed;
            Despawn(false);
            CancellationTokenSourceUtility.Collect(ref _destroyCancellationTokenSource);

            if (IsQuitting)
            {
                OnDestroying?.Invoke(this, DestroyReason.ApplicationQuit);
            }
            else if (!GameObject.scene.isLoaded)
            {
                OnDestroying?.Invoke(this, DestroyReason.SceneChange);
            }
            else
            {
                OnDestroying?.Invoke(this, DestroyReason.ExplicitCall);
            }

            OnDestroyed();

            if (_operationHandle.IsValid())
            {
                Addressables.ReleaseInstance(_operationHandle);
            }

            if (callObjectDestroy)
            {
                if (CoimbraUtility.IsPlayMode)
                {
#pragma warning disable COIMBRA0008
                    Object.Destroy(gameObject);
#pragma warning restore COIMBRA0008
                }
                else
                {
                    DestroyImmediate(gameObject);
                }
            }

            OnActiveStateChanged = null;
            OnDestroying = null;
            Pool = null;
            _gameObject = null;
            _transform = null;
            InitializedActorCount.Value--;
            CachedActors.Remove(GameObjectID);
        }

        private void InitializeComponentsAndSelf()
        {
            using (ListPool.Pop(out List<ActorComponentBase> components))
            {
                GetComponents(components);

                foreach (ActorComponentBase component in components)
                {
                    component.PreInitialize(this);

                    if (IsDestroyed)
                    {
                        return;
                    }
                }

                OnInitialize();

                foreach (ActorComponentBase component in components)
                {
                    if (IsDestroyed)
                    {
                        return;
                    }

                    component.PostInitialize();
                }
            }
        }
    }
}
