using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using Object = UnityEngine.Object;

namespace Coimbra
{
    /// <summary>
    /// Used to represent a <see cref="GameObject"/>. It is expected to inherit from this class to create the main script of each object.
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
        /// Invoked each time a new <see cref="Scene"/> is loaded and all of its <see cref="Actor"/> got initialized. Use <see cref="OnSceneInitializedOnce"/> if you need to only fire it once.
        /// </summary>
        public static event UnityAction<Scene, LoadSceneMode> OnSceneInitialized;

        /// <summary>
        /// Invoked each time a new <see cref="Scene"/> is loaded and all of its <see cref="Actor"/> got initialized. It resets after each call and is called after <see cref="OnSceneInitialized"/>.
        /// </summary>
        public static event UnityAction<Scene, LoadSceneMode> OnSceneInitializedOnce;

        /// <summary>
        /// Invoked when a <see cref="GameObject"/> is activated or deactivated in the scene.
        /// </summary>
        public event ActiveStateHandler OnActiveStateChanged;

        /// <summary>
        /// Invoked when a <see cref="GameObject"/> is being destroyed for any reason.
        /// </summary>
        public event DestroyHandler OnDestroying;

        private static readonly List<Actor> PooledActors = new();

        private static readonly List<WeakReference<Actor>> UninitializedActors = new();

        private static readonly Dictionary<GameObjectID, Actor> CachedActors = new();

        private bool _isUnloadingScene;

        private GameObjectID? _gameObjectID;

        private AsyncOperationHandle<GameObject> _operationHandle;

        private CancellationTokenSource _despawnCancellationTokenSource;

        private CancellationTokenSource _destroyCancellationTokenSource;

        protected Actor()
        {
            UninitializedActors.Add(new WeakReference<Actor>(this));
        }

        /// <summary>
        /// Cached version of <see cref="MonoBehaviour.gameObject"/>.<see cref="Object.GetInstanceID"/>.
        /// </summary>
        public GameObjectID GameObjectID => _gameObjectID ?? (_gameObjectID = gameObject).Value;

        /// <summary>
        /// Cached version of <see cref="MonoBehaviour.gameObject"/> to avoid the C++ interop.
        /// </summary>
        public GameObject CachedGameObject { get; private set; }

        /// <summary>
        /// Cached version of <see cref="MonoBehaviour.transform"/> to avoid the C++ interop.
        /// </summary>
        public Transform CachedTransform { get; private set; }

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
        /// Was <see cref="Destroy"/> called at least once in this <see cref="Actor"/> or <see cref="GameObject"/>?
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
        [field: SerializeField]
        [field: Disable]
        [field: Tooltip("The pool that owns this instance.")]
        public GameObjectPool Pool { get; private set; }

        /// <summary>
        /// If true, it will deactivate the prefab when initializing it.
        /// </summary>
        [field: SerializeField]
        [field: DisableOnPlayMode]
        [field: Tooltip("If true, it will deactivate the prefab when initializing it.")]
        public bool DeactivatePrefabOnInitialize { get; set; }

        /// <summary>
        /// If true, it will activate the object when spawning it.
        /// </summary>
        [field: SerializeField]
        [field: DisableOnPlayMode]
        [field: Tooltip("If true, it will activate the object when spawning it.")]
        public bool ActivateOnSpawn { get; set; }

        /// <summary>
        /// If true, it will deactivate the object when despawning it.
        /// </summary>
        [field: SerializeField]
        [field: DisableOnPlayMode]
        [field: Tooltip("If true, it will deactivate the object when despawning it.")]
        public bool DeactivateOnDespawn { get; set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator GameObject(Actor actor)
        {
            return actor.CachedGameObject;
        }

        /// <summary>
        /// Initialize all uninitialized actors. This is called for each <see cref="SceneManager.sceneLoaded"/> but can also be used after instantiating any <see cref="Actor"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InitializeActors()
        {
            while (UninitializedActors.Count > 0)
            {
                if (UninitializedActors.Pop().TryGetTarget(out Actor actor) && actor.TryGetValid(out actor))
                {
                    actor.Initialize();
                }
            }
        }

        /// <summary>
        /// Is the <see cref="Actor"/> representation of specified <see cref="GameObject"/> cached?
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasCachedActor(GameObject gameObject, out Actor actor)
        {
            return CachedActors.TryGetValue(gameObject, out actor);
        }

        /// <summary>
        /// Despawns the <see cref="GameObject"/> and return it to its pool. If it doesn't belong to a <see cref="GameObjectPool"/>, it will <see cref="Destroy"/> the object instead.
        /// </summary>
        public void Despawn()
        {
            Despawn(true);
        }

        /// <summary>
        /// Destroys the <see cref="GameObject"/> that this actor represents.
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
        /// Should be called externally when a <see cref="Scene"/> is about to be unloaded. Called by default on <see cref="CoimbraSceneManagerAPI"/>.
        /// </summary>
        /// <param name="scene"></param>
        public void OnUnloadScene(Scene scene)
        {
            if (CachedGameObject.scene == Pool.CachedGameObject.scene)
            {
                return;
            }

            if (CachedGameObject.scene == scene)
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
                    CachedTransform.SetParent(Pool.CachedTransform, false);
                }

                return;
            }

            if (Pool.CachedGameObject.scene == scene)
            {
                Pool = null;
            }
        }

        /// <summary>
        /// Called each time this object is despawned. By default, it deactivates the object.
        /// </summary>
        protected virtual void OnDespawn()
        {
            if (DeactivateOnDespawn)
            {
                CachedGameObject.SetActive(false);
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
            if (DeactivatePrefabOnInitialize)
            {
                CachedGameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Called each time this object is spawned. By default, it activates the object.
        /// </summary>
        protected virtual void OnSpawn()
        {
            if (ActivateOnSpawn)
            {
                CachedGameObject.SetActive(true);
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Non-virtual by design, use <see cref="OnInitialize"/> instead.
        /// </summary>
        protected void Awake()
        {
            Debug.Assert(true);
        }
#endif

#if UNITY_ASSERTIONS
        /// <summary>
        /// Non-virtual by design, use <see cref="OnInitialize"/> instead.
        /// </summary>
        protected void Start()
        {
            Debug.Assert(IsInitialized, $"{nameof(Actor)}.{nameof(Initialize)} needs to be called before the {nameof(Start)} callback!", this);
        }
#endif

        /// <summary>
        /// Non-virtual by design, use <see cref="OnActiveStateChanged"/> instead.
        /// </summary>
        protected void OnEnable()
        {
            OnActiveStateChanged?.Invoke(this, true);
        }

        /// <summary>
        /// Non-virtual by design, use <see cref="OnActiveStateChanged"/> instead.
        /// </summary>
        protected void OnDisable()
        {
            OnActiveStateChanged?.Invoke(this, false);
        }

        /// <summary>
        /// Non-virtual by design, use <see cref="OnDestroyed"/> instead.
        /// </summary>
        protected void OnDestroy()
        {
            CachedGameObject = gameObject;
            CachedTransform = transform;
            Destroy(false);
        }

        /// <summary>
        /// Non-virtual by design, use <see cref="Application.quitting"/> instead.
        /// </summary>
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
            if (IsDestroyed || IsInitialized)
            {
                return;
            }

            Pool = pool;
            IsInitialized = true;
            CachedTransform = transform;
            CachedGameObject = gameObject;
            IsPrefab = CachedGameObject.scene.name == null;
            _operationHandle = operationHandle;
            _gameObjectID = CachedGameObject;
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
                PooledActors.Add(this);
            }
            else
            {
                Spawn();
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
                PooledActors.RemoveSwapBack(this);
            }

            IsDestroyed = true;
            Despawn(false);
            CancellationTokenSourceUtility.Collect(ref _destroyCancellationTokenSource);

            if (IsQuitting)
            {
                OnDestroying?.Invoke(this, DestroyReason.ApplicationQuit);
            }
            else if (_isUnloadingScene || !CachedGameObject.scene.isLoaded)
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
                Object.Destroy(CachedGameObject);
            }

            OnActiveStateChanged = null;
            OnDestroying = null;
            CachedActors.Remove(GameObjectID);
            CachedGameObject = null;
            CachedTransform = null;
            Pool = null;
        }
    }
}
