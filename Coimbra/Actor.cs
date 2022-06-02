using JetBrains.Annotations;
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
    public class Actor : MonoBehaviour
    {
        /// <summary>
        /// Defines the reason why the object is being destroyed.
        /// </summary>
        public enum DestroyReason
        {
            /// <summary>
            /// Instigated by an user code explicitly.
            /// </summary>
            ExplicitCall,

            /// <summary>
            /// Instigated by a scene change when the object was not flagged to don't destroy on load.
            /// </summary>
            SceneChange,

            /// <summary>
            /// Being called due the application being shutdown.
            /// </summary>
            ApplicationQuit,
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

        private static readonly List<Actor> PooledActors = new List<Actor>();

        private static readonly List<Actor> UninitializedActors = new List<Actor>();

        private static readonly Dictionary<GameObjectID, Actor> CachedActors = new Dictionary<GameObjectID, Actor>();

        private static readonly ProfilerCounterValue<int> NewActorCount = new ProfilerCounterValue<int>(CoimbraUtility.ProfilerCategory, "New Actors", ProfilerMarkerDataUnit.Count, ProfilerCounterOptions.ResetToZeroOnFlush);

        private static readonly ProfilerCounterValue<int> PooledActorCount = new ProfilerCounterValue<int>(CoimbraUtility.ProfilerCategory, "Pooled Actors", ProfilerMarkerDataUnit.Count, ProfilerCounterOptions.FlushOnEndOfFrame);

        private static readonly ProfilerCounterValue<int> InitializedActorCount = new ProfilerCounterValue<int>(CoimbraUtility.ProfilerCategory, "Initialized Actors", ProfilerMarkerDataUnit.Count, ProfilerCounterOptions.FlushOnEndOfFrame);

        private static readonly ProfilerCounterValue<int> UninitializedActorCount = new ProfilerCounterValue<int>(CoimbraUtility.ProfilerCategory, "Uninitialized Actors", ProfilerMarkerDataUnit.Count, ProfilerCounterOptions.FlushOnEndOfFrame);

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

        [SerializeField]
        [DisableOnPlayMode]
        [FormerlySerializedAsBackingFieldOf("DeactivatePrefabOnInitialize")]
        [Tooltip("If true, it will deactivate the prefab when initializing it.")]
        private bool _deactivateOnInitializePrefab;

        private bool _isUnloadingScene;

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
        /// If true, it will activate the object when spawning it.
        /// </summary>
        public bool ActivateOnSpawn
        {
            [DebuggerStepThrough]
            get => _activateOnSpawn;
            [DebuggerStepThrough]
            set => _activateOnSpawn = value;
        }

        /// <summary>
        /// If true, it will check if <see cref="Awake"/> was called before <see cref="Initialize"/>.
        /// </summary>
        public bool AssertAwake
        {
            [DebuggerStepThrough]
            get => _assertAwake;
            [DebuggerStepThrough]
            set => _assertAwake = value;
        }

        /// <summary>
        /// If true, it will deactivate the object when despawning it.
        /// </summary>
        public bool DeactivateOnDespawn
        {
            [DebuggerStepThrough]
            get => _deactivateOnDespawn;
            [DebuggerStepThrough]
            set => _deactivateOnDespawn = value;
        }

        /// <summary>
        /// If true, it will deactivate the prefab when initializing it.
        /// </summary>
        public bool DeactivateOnInitializePrefab
        {
            [DebuggerStepThrough]
            get => _deactivateOnInitializePrefab;
            [DebuggerStepThrough]
            set => _deactivateOnInitializePrefab = value;
        }

        /// <summary>
        /// <see cref="CancellationToken"/> for when this <see cref="Actor"/> is about to be despawned.
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
        /// <see cref="CancellationToken"/> for when this <see cref="Actor"/> is about to be destroyed.
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
        /// Cached version of <see cref="MonoBehaviour.gameObject"/>.<see cref="Object.GetInstanceID"/>.
        /// </summary>
        public GameObjectID GameObjectID => _gameObjectID ?? (_gameObjectID = GameObject).Value;

        /// <summary>
        /// Cached version of <see cref="MonoBehaviour.gameObject"/> to avoid the C++ interop.
        /// </summary>
        public GameObject GameObject => IsDestroyed || !IsInitialized ? gameObject : _gameObject;

        /// <summary>
        /// Cached version of <see cref="MonoBehaviour.transform"/> to avoid the C++ interop.
        /// </summary>
        public Transform Transform => IsDestroyed || !IsInitialized ? transform : _transform;

        /// <summary>
        /// True if <see cref="Awake"/> was called already.
        /// </summary>
        public bool IsAwaken { get; private set; }

        /// <summary>
        /// Was <see cref="Destroy"/> called at least once in this <see cref="Actor"/> or <see cref="UnityEngine.GameObject"/>?
        /// </summary>
        public bool IsDestroyed { get; private set; }

        /// <summary>
        /// Was <see cref="Initialize"/> called at least once in this <see cref="Actor"/>?
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Indicates if the object was instantiated through a <see cref="GameObjectPool"/>.
        /// </summary>
        public bool IsPooled { get; private set; }

        /// <summary>
        /// True when this object is a prefab asset.
        /// </summary>
        public bool IsPrefab { get; private set; }

        /// <summary>
        /// True when application is quitting.
        /// </summary>
        public bool IsQuitting { get; private set; }

        /// <summary>
        /// Indicates if the object is currently spawned.
        /// </summary>
        public bool IsSpawned { get; private set; }

        /// <summary>
        /// The pool that owns this instance.
        /// </summary>
        public GameObjectPool Pool { get; private set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator GameObject(Actor actor)
        {
            return actor.GameObject;
        }

        /// <summary>
        /// Is the <see cref="Actor"/> representation of specified <see cref="UnityEngine.GameObject"/> cached?
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasCachedActor(GameObject gameObject, out Actor actor)
        {
            return CachedActors.TryGetValue(gameObject, out actor);
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

        /// <summary>
        /// Initializes this actor. It will also spawn it if not <see cref="IsPooled"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Initialize()
        {
            Initialize(null, default);
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
        protected virtual void OnInitializePrefab()
        {
            if (_deactivateOnInitializePrefab)
            {
                GameObject.SetActive(false);
            }
        }

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
        /// Non-virtual by design, use <see cref="OnInitialize"/> instead.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void Awake()
        {
            if (!IsAwaken)
            {
                IsAwaken = isActiveAndEnabled;
            }
        }

#if UNITY_ASSERTIONS
        /// <summary>
        /// Non-virtual by design, use <see cref="OnInitialize"/> and wait one frame, or listen to <see cref="OnSceneInitializedOnce"/> instead.
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
            IsQuitting = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static IReadOnlyList<Actor> GetPooledActors()
        {
            return PooledActors;
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
            IsInitialized = true;

            // should be initialized already to work correctly
            _gameObjectID = GameObject;
            IsPrefab = GameObject.scene.name == null;

            // should be the last call
            InitializedActorCount.Value++;
            CachedActors.Add(GameObjectID, this);

            if (IsPrefab)
            {
                OnInitializePrefab();

                return;
            }

            IsPooled = Pool != null;
            OnInitialize();

            if (IsPooled)
            {
                PooledActorCount.Value++;
                PooledActors.Add(this);
            }
            else
            {
                Spawn();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnUnloadScene(Scene scene)
        {
            if (GameObject.scene == Pool.GameObject.scene)
            {
                return;
            }

            if (GameObject.scene == scene)
            {
                _isUnloadingScene = true;
                Despawn();

                if (IsDestroyed)
                {
                    return;
                }

                _isUnloadingScene = false;

                if (Pool.KeepParentOnDespawn)
                {
                    Transform.SetParent(Pool.Transform, false);
                }

                return;
            }

            if (Pool.GameObject.scene == scene)
            {
                Pool = null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Spawn()
        {
            if (IsDestroyed || IsSpawned)
            {
                return;
            }

            IsSpawned = true;
            OnSpawn();
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

            IsSpawned = false;
            CancellationTokenSourceUtility.Collect(ref _despawnCancellationTokenSource);
            OnDespawn();

            if (!callPoolDespawnOrDestroy)
            {
                return;
            }

            if (IsPooled && Pool != null && Pool.CurrentState != GameObjectPool.State.Unloaded)
            {
                Pool.Despawn(this);
            }
            else
            {
                Destroy(true);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Destroy(bool callObjectDestroy)
        {
            if (IsDestroyed)
            {
                return;
            }

            if (IsPooled)
            {
                PooledActorCount.Value--;
                PooledActors.RemoveSwapBack(this);
            }

            IsDestroyed = true;
            Despawn(false);
            CancellationTokenSourceUtility.Collect(ref _destroyCancellationTokenSource);

            if (IsQuitting)
            {
                OnDestroying?.Invoke(this, DestroyReason.ApplicationQuit);
            }
            else if (_isUnloadingScene || !gameObject.scene.isLoaded)
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
                    Object.Destroy(gameObject);
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
    }
}
