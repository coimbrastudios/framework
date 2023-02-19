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
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Coimbra
{
    /// <summary>
    /// Used to represent a <see cref="GameObject"/>. You can inherit from this class to create the main script of each object.
    /// </summary>
    /// <remarks>
    /// Different from most <see cref="MonoBehaviour"/>, the <see cref="Actor"/> contains the <see cref="DisallowMultipleComponent"/> and is expected to have its lifecycle completely linked to the <see cref="GameObject"/> it is attached to. Any <see cref="GameObject"/> without an explicit <see cref="Actor"/> type attached is implicitly an <see cref="Actor"/> by default.
    /// <para></para>
    /// It encapsulates some ambiguous Unity callbacks to ensure that they are actually fired in the expected timings, this includes the <see cref="OnInitialize"/> method that will get fired even if the objects never gets activated and its <see cref="OnDestroyed"/> method that will get fired even if the object was never set active during its lifecycle.
    /// <para></para>
    /// To achieve that, custom APIs are provided to ensure proper creation and destruction for any given <see cref="Actor"/>. Take a look into <see cref="Initialize"/>, that should be called after each instantiation (check documentation for alternatives), and <see cref="Dispose"/>, that should be used instead of the static <see cref="Object.Destroy(Object)"/> method.
    /// <para></para>
    /// It offers additional <see cref="GameObject"/>-level events (<see cref="OnStarting"/>, <see cref="OnActiveStateChanged"/>, and <see cref="OnDestroying"/>), and built-in pooling support with the <see cref="OnSpawn"/> and <see cref="OnDispose"/> APIs (<see cref="GameObjectPool"/>).
    /// <para></para>
    /// It also offers some common cancellation tokens (<see cref="DisposeCancellationToken"/> and <see cref="DestroyCancellationToken"/>), cached properties for <see cref="Transform"/>, <see cref="GameObject"/>, and <see cref="GameObjectID"/>, useful <see cref="StateFlags"/>, and implements the <see cref="IDisposable"/> interface.
    /// </remarks>
    /// <seealso cref="ActorComponentBase"/>
    /// <seealso cref="GameObjectID"/>
    /// <seealso cref="GameObjectPool"/>
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
            /// <see cref="Actor.IsStarted"/>.
            /// </summary>
            IsStarted = 1 << 2,

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
        /// Delegate for handler a <see cref="GameObject"/> start.
        /// </summary>
        public delegate void StartHandler(Actor sender);

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
        /// Invoked each time a new <see cref="Scene"/> is loaded and all of its <see cref="Actor"/> got initialized. Use <see cref="OnSceneActorsInitializedOnce"/> if you need to only fire it once.
        /// </summary>
        public static event SceneInitializedHandler OnSceneActorsInitialized;

        /// <summary>
        /// Invoked each time a new <see cref="Scene"/> is loaded and all of its <see cref="Actor"/> got initialized. It resets after each call and is called after <see cref="OnSceneActorsInitialized"/>.
        /// </summary>
        public static event SceneInitializedHandler OnSceneActorsInitializedOnce;

        /// <summary>
        /// Invoked when a <see cref="UnityEngine.GameObject"/> is activated or deactivated in the scene.
        /// </summary>
        public event ActiveStateHandler OnActiveStateChanged;

        /// <summary>
        /// Invoked when a <see cref="UnityEngine.GameObject"/> is being destroyed for any reason.
        /// </summary>
        public event DestroyHandler OnDestroying
        {
            add
            {
                Debug.Assert(!IsDestroyed, $"{nameof(OnDestroying)} shouldn't receive new listeners after the object is destroyed!", this);
                _onDestroying += value;
            }

            [DebuggerStepThrough]
            remove => _onDestroying -= value;
        }

        /// <summary>
        /// Invoked when a <see cref="UnityEngine.GameObject"/> is being started.
        /// </summary>
        public event StartHandler OnStarting
        {
            add
            {
                Debug.Assert(!IsStarted, $"{nameof(OnStarting)} shouldn't receive new listeners after the object is started!", this);
                _onStarting += value;
            }

            [DebuggerStepThrough]
            remove => _onStarting -= value;
        }

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

        [FormerlySerializedAs("_deactivateOnDespawn")]
        [SerializeField]
        [DisableOnPlayMode]
        [FormerlySerializedAsBackingFieldOf("DeactivateOnDispose")]
        [Tooltip("If true, it will deactivate the object when disposing it.")]
        private bool _deactivateOnDispose;

        private GameObjectID? _gameObjectID;

        private AsyncOperationHandle<GameObject> _operationHandle;

        private CancellationTokenSource _disposeCancellationTokenSource;

        private CancellationTokenSource _destroyCancellationTokenSource;

        private GameObject _gameObject;

        private Transform _transform;

        private DestroyHandler _onDestroying;

        private StartHandler _onStarting;

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
        /// Gets or sets a value indicating whether it will deactivate the object when disposing it.
        /// </summary>
        public bool DeactivateOnDispose
        {
            [DebuggerStepThrough]
            get => _deactivateOnDispose;
            [DebuggerStepThrough]
            set => _deactivateOnDispose = value;
        }

        /// <summary>
        /// Gets the <see cref="CancellationToken"/> for when this <see cref="Actor"/> is about to be disposed.
        /// </summary>
        public CancellationToken DisposeCancellationToken
        {
            get
            {
                if (!IsSpawned)
                {
                    return CancellationToken.None;
                }

                _disposeCancellationTokenSource ??= new CancellationTokenSource();

                return _disposeCancellationTokenSource.Token;
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
        /// Gets a value indicating whether <see cref="Dispose"/> was called at least once in this <see cref="Actor"/> or <see cref="UnityEngine.GameObject"/>.
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
        public bool IsPrefab => GameObject.scene.name == null;

        /// <summary>
        /// Gets a value indicating whether the application is quitting.
        /// </summary>
        public bool IsQuitting => (States & StateFlags.IsQuitting) != 0;

        /// <summary>
        /// Gets a value indicating whether the object is currently spawned.
        /// </summary>
        public bool IsSpawned => (States & StateFlags.IsSpawned) != 0;

        /// <summary>
        /// Gets a value indicating whether <see cref="Start"/> was called already.
        /// </summary>
        public bool IsStarted => (States & StateFlags.IsStarted) != 0;

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
        /// Destroys the <see cref="UnityEngine.GameObject"/> that this actor represents. By default, will return it to its <see cref="GameObjectPool"/>, unless it doesn't belong to one.
        /// </summary>
        /// <returns>
        /// <list type="bullet">
        /// <item><see cref="ObjectDisposeResult.None"/>When <see cref="IsDestroyed"/> was already true, or when <see cref="IsSpawned"/> was already false and <paramref name="forceDestroy"/> was false.</item>
        /// <item><see cref="ObjectDisposeResult.Pooled"/>When was pushed back to the <see cref="GameObjectPool"/> with success.</item>
        /// <item><see cref="ObjectDisposeResult.Destroyed"/>When failed to be pushed back to the <see cref="GameObjectPool"/> or <paramref name="forceDestroy"/> was true.</item>
        /// </list>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectDisposeResult Dispose(bool forceDestroy)
        {
            if (forceDestroy)
            {
                return DestroyInstance(true) ? ObjectDisposeResult.Destroyed : ObjectDisposeResult.None;
            }

            return ReturnInstance(true);
        }

        /// <summary>
        /// Initializes this actor. It will also spawn it if not <see cref="IsPooled"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [ContextMenu(nameof(Initialize))]
        public void Initialize()
        {
            Initialize(null, default(AsyncOperationHandle<GameObject>));
        }

        internal static IReadOnlyCollection<Actor> GetCachedActors()
        {
            return CachedActors.Values;
        }

        internal static void OnPlayModeStateChanged()
        {
            OnSceneActorsInitialized = null;
            OnSceneActorsInitializedOnce = null;
            InitializedActorCount.Value = 0;
            UninitializedActorCount.Value = 0;
            UninitializedActors.Clear();
            CachedActors.Clear();

            foreach (Actor actor in FindObjectsOfType<Actor>(true))
            {
                if (actor.IsInitialized)
                {
                    CachedActors.Add(actor.GameObjectID, actor);
                }
            }
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

            _gameObjectID = GameObject;
            UninitializedActorCount.Value--;
            InitializedActorCount.Value++;
            CachedActors.Add(GameObjectID, this);

            if (IsPrefab)
            {
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
            if (IsDestroyed)
            {
                return;
            }

            States |= StateFlags.IsUnloadingScene;

            if (!IsPooled)
            {
                if (!IsAwaken)
                {
                    Dispose(true);
                }

                return;
            }

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

            Dispose(false);

            if (GameObject.scene != scene)
            {
                States &= ~StateFlags.IsUnloadingScene;

                return;
            }

            if (!Pool.KeepParentWhenReturning || Pool.GameObject.scene == scene)
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
        /// Use this for one-time un-initializations instead of <see cref="OnDestroy"/> callback. This method is called even if the object starts inactive.
        /// </summary>
        protected virtual void OnDestroyed() { }

        /// <summary>
        /// Called each time this object is disposed. By default, it deactivates the object.
        /// </summary>
        protected virtual void OnDispose()
        {
            if (DeactivateOnDispose)
            {
                GameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Use this for one-time initializations instead of <see cref="Awake"/> callback. This method is called even if the object starts inactive.
        /// </summary>
        protected virtual void OnInitialize() { }

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
        protected virtual void OnValidate() { }

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
            _onStarting?.Invoke(this);
            _onStarting = null;
            States |= StateFlags.IsStarted;
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
            DestroyInstance(false);
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
            OnSceneActorsInitialized?.Invoke(scene, loadSceneMode);

            if (OnSceneActorsInitializedOnce == null)
            {
                return;
            }

            OnSceneActorsInitializedOnce.Invoke(scene, loadSceneMode);
            OnSceneActorsInitializedOnce = null;
        }

        [ContextMenu(nameof(Dispose), true)]
        private bool CanDispose()
        {
            return IsSpawned;
        }

        [ContextMenu("Dispose (Force Destroy)", true)]
        private bool CanDisposeWithForceDestroy()
        {
            return !IsDestroyed;
        }

        [ContextMenu(nameof(Initialize), true)]
        private bool CanInitialize()
        {
            return !IsInitialized;
        }

        [ContextMenu("Dispose (Force Destroy)")]
        private void DisposeWithForceDestroy()
        {
            Dispose(true);
        }

        private bool DestroyInstance(bool callObjectDestroy)
        {
            if (IsDestroyed)
            {
                return false;
            }

            States |= StateFlags.IsDestroyed;
            ReturnInstance(false);
            CancellationTokenSourceUtility.Collect(ref _destroyCancellationTokenSource);

            if (IsQuitting)
            {
                _onDestroying?.Invoke(this, DestroyReason.ApplicationQuit);
            }
            else if (!GameObject.scene.isLoaded)
            {
                _onDestroying?.Invoke(this, DestroyReason.SceneChange);
            }
            else
            {
                _onDestroying?.Invoke(this, DestroyReason.ExplicitCall);
            }

            OnDestroyed();

            if (_operationHandle.IsValid())
            {
                Addressables.ReleaseInstance(_operationHandle);
            }

            if (callObjectDestroy)
            {
                if (ApplicationUtility.IsPlayMode)
                {
#pragma warning disable COIMBRA0008
                    Destroy(gameObject);
#pragma warning restore COIMBRA0008
                }
                else
                {
                    DestroyImmediate(gameObject);
                }
            }

            OnActiveStateChanged = null;
            Pool = null;
            _onDestroying = null;
            _gameObject = null;
            _transform = null;
            InitializedActorCount.Value--;
            CachedActors.Remove(GameObjectID);

            return true;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ObjectDisposeResult ReturnInstance(bool callPoolReturnOrDestroyInstance)
        {
            if (!IsSpawned)
            {
                return ObjectDisposeResult.None;
            }

            States |= StateFlags.IsSpawned;
            CancellationTokenSourceUtility.Collect(ref _disposeCancellationTokenSource);
            OnDispose();

            if (!callPoolReturnOrDestroyInstance)
            {
                return ObjectDisposeResult.None;
            }

            if (IsPooled && Pool != null && Pool.ReturnInstance(this))
            {
                return ObjectDisposeResult.Pooled;
            }

            return DestroyInstance(true) ? ObjectDisposeResult.Destroyed : ObjectDisposeResult.None;
        }

        /// <inheritdoc/>
        [ContextMenu(nameof(Dispose))]
        void IDisposable.Dispose()
        {
            Dispose(false);
        }
    }
}
