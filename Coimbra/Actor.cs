using JetBrains.Annotations;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Used to represent a <see cref="GameObject"/>. It is expected to inherit from this class to create the main script of each object.
    /// </summary>
    [PublicAPI]
    [Preserve]
    [DisallowMultipleComponent]
    [AddComponentMenu(FrameworkUtility.GeneralMenuPath + "Actor")]
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
        /// Invoked when a <see cref="GameObject"/> is activated or deactivated in the scene.
        /// </summary>
        public event ActiveStateHandler OnActiveStateChanged;

        /// <summary>
        /// Invoked when a <see cref="GameObject"/> is destroyed for any reason.
        /// </summary>
        public event DestroyHandler OnDestroyed;

        private static readonly List<Actor> ConstructedActors = new List<Actor>();

        private static readonly Dictionary<GameObjectID, Actor> CachedActors = new Dictionary<GameObjectID, Actor>();

        private GameObjectID? _gameObjectID;

        protected Actor()
        {
            ConstructedActors.Add(this);
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
        public GameObjectPool Pool { get; internal set; }

        internal AsyncOperationHandle<GameObject> OperationHandle { get; set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator GameObject(Actor actor)
        {
            return actor.CachedGameObject;
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
        /// Initializes all currently constructed <see cref="Actor"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InitializeAllActors()
        {
            for (int i = ConstructedActors.Count - 1; i >= 0; i--)
            {
                Actor actor = ConstructedActors[i];

                if (actor != null)
                {
                    actor.Initialize();
                }

                ConstructedActors.RemoveAt(i);
            }
        }

        /// <summary>
        /// Despawns the <see cref="GameObject"/> and return it to its pool. If it doesn't belong to a <see cref="GameObjectPool"/>, it will <see cref="Destroy"/> the object instead.
        /// </summary>
        public void Despawn()
        {
            if (!IsSpawned)
            {
                return;
            }

            IsSpawned = false;
            OnDespawn();

            if (IsPooled && Pool != null && Pool.CurrentState != GameObjectPool.State.Unloaded)
            {
                Pool.Despawn(this);
            }
            else
            {
                Destroy(true);
            }
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
        public void Initialize()
        {
            if (IsDestroyed || IsInitialized)
            {
                return;
            }

            IsInitialized = true;
            CachedTransform = transform;
            CachedGameObject = gameObject;
            IsPrefab = CachedGameObject.scene.name == null;
            _gameObjectID = CachedGameObject;
            CachedActors.Add(GameObjectID, this);

            if (IsPrefab)
            {
                OnInitializePrefab();

                return;
            }

            IsPooled = Pool != null;
            OnInitialize();

            if (!IsPooled)
            {
                Spawn();
            }
        }

        /// <summary>
        /// Called each time this object is despawned.
        /// </summary>
        protected virtual void OnDespawn() { }

        /// <summary>
        /// Use this for one-time un-initializations instead of OnDestroy callback. This method is called even if the object starts inactive.
        /// <para>This method will be the first thing to happen when calling <see cref="Destroy"/> manually, but will also happen inside OnDestroy if using <see cref="Object.Destroy(Object)"/>.</para>
        /// </summary>
        protected virtual void OnDestroying() { }

        /// <summary>
        /// Use this for one-time initializations instead of Awake callback. This method is called even if the object starts inactive.
        /// </summary>
        protected virtual void OnInitialize() { }

        /// <summary>
        /// Use this for one-time initializations on prefabs.
        /// </summary>
        protected virtual void OnInitializePrefab() { }

        /// <summary>
        /// Called each time this object is spawned.
        /// </summary>
        protected virtual void OnSpawn() { }

        /// <summary>
        /// Non-virtual by design, use <see cref="OnInitialize"/> instead.
        /// </summary>
        protected void Awake()
        {
            Initialize();
        }

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
        /// Non-virtual by design, use <see cref="OnDestroying"/> instead.
        /// </summary>
        protected void OnDestroy()
        {
            CachedGameObject = gameObject;
            CachedTransform = transform;
            Destroy(false);
        }

        /// <summary>
        /// Non-virtual by design, use <see cref="ApplicationQuitEvent"/> instead.
        /// </summary>
        protected void OnApplicationQuit()
        {
            IsQuitting = true;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Destroy(bool callDestroy)
        {
            if (IsDestroyed)
            {
                return;
            }

            IsDestroyed = true;

            if (IsSpawned)
            {
                IsSpawned = false;
                OnDespawn();
            }

            OnDestroying();

            if (IsQuitting)
            {
                OnDestroyed?.Invoke(this, DestroyReason.ApplicationQuit);
            }
            else if (CachedGameObject.scene.isLoaded)
            {
                OnDestroyed?.Invoke(this, DestroyReason.ExplicitCall);
            }
            else
            {
                OnDestroyed?.Invoke(this, DestroyReason.SceneChange);
            }

            if (OperationHandle.IsValid())
            {
                Addressables.ReleaseInstance(OperationHandle);
            }

            if (callDestroy)
            {
                Object.Destroy(CachedGameObject);
            }

            OnActiveStateChanged = null;
            OnDestroyed = null;
            CachedActors.Remove(GameObjectID);
            CachedGameObject = null;
            CachedTransform = null;
            Pool = null;
        }
    }
}
