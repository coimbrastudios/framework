using JetBrains.Annotations;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Scripting;

namespace Coimbra
{
    [PublicAPI]
    [Preserve]
    [DisallowMultipleComponent]
    [AddComponentMenu(FrameworkUtility.GeneralMenuPath + "GameObject Behaviour")]
    public class GameObjectBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Delegate for handling a <see cref="GameObject"/> active state changes.
        /// </summary>
        public delegate void ActiveStateHandler(GameObjectBehaviour sender, bool state);

        /// <summary>
        /// Delegate for handling a <see cref="GameObject"/> destroy.
        /// </summary>
        public delegate void DestroyHandler(GameObjectBehaviour sender, DestroyReason reason);

        /// <summary>
        /// Invoked when a <see cref="GameObject"/> is activated or deactivated in the scene.
        /// </summary>
        public event ActiveStateHandler OnActiveStateChanged;

        /// <summary>
        /// Invoked when a <see cref="GameObject"/> is destroyed for any reason.
        /// </summary>
        public event DestroyHandler OnDestroyed;

        private bool _isDestroyed;
        private bool _isInitialized;
        private bool _isQuitting;
        private GameObjectID? _gameObjectID;

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
        /// Indicates if the object belongs to a pool as <see cref="Pool"/> can already be null due a scene change.
        /// </summary>
        public bool IsPooled { get; private set; }

        /// <summary>
        /// Indicates if the object is currently spawned or should be treated as non-spawned.
        /// </summary>
        public bool IsSpawned { get; private set; }

        /// <summary>
        /// The pool that owns this instance.
        /// </summary>
        [field: SerializeField]
        [field: Disable]
        public GameObjectPool Pool { get; internal set; }

        public static implicit operator GameObject(GameObjectBehaviour behaviour)
        {
            return behaviour.CachedGameObject;
        }

        /// <summary>
        /// Destroys the <see cref="GameObject"/> that this behaviour belongs to.
        /// </summary>
        public void Destroy()
        {
            Destroy(true);
        }

        /// <summary>
        /// Initializes this behaviour.
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            _isInitialized = true;
            IsPooled = Pool != null;
            CachedGameObject = gameObject;
            CachedTransform = transform;
            _gameObjectID = CachedGameObject;
            GameObjectUtility.AddCachedBehaviour(this);
            OnObjectInitialize();
        }

        /// <summary>
        /// Non-virtual by design, use <see cref="OnObjectInitialize"/> instead.
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
        /// Non-virtual by design, use <see cref="OnObjectDestroy"/> instead.
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
            _isQuitting = true;
        }

        /// <summary>
        /// Called each time this object is despawned.
        /// <para>By default it deactivates the <see cref="GameObject"/>, so consider calling the base method at the end.</para>
        /// </summary>
        protected virtual void OnObjectDespawn()
        {
            CachedGameObject.SetActive(false);
        }

        /// <summary>
        /// Use this for one-time un-initializations instead of OnDestroy callback. This method is called even if the object starts inactive.
        /// <para>By default it calls <see cref="Despawn"/>, so consider calling the base method at the begin.</para>
        /// </summary>
        protected virtual void OnObjectDestroy()
        {
            Despawn();
        }

        /// <summary>
        /// Use this for one-time initializations instead of Awake callback. This method is called even if the object starts inactive.
        /// </summary>
        /// <para>By default it calls <see cref="Spawn"/> if non-pooled or if pooled but already spawned, so consider calling the base method at the end.</para>
        protected virtual void OnObjectInitialize()
        {
            if (!IsPooled || !Pool.Contains(GameObjectID))
            {
                Spawn();
            }
        }

        /// <summary>
        /// Called each time this object is spawned.
        /// </summary>
        /// <para>By default it activates the <see cref="GameObject"/>, so consider calling the base method at the begin.</para>
        protected virtual void OnObjectSpawn()
        {
            CachedGameObject.SetActive(true);
        }

        /// <summary>
        /// Wrapper for <see cref="OnObjectDespawn"/> that checks and toggles the <see cref="IsSpawned"/> flag.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal void Despawn()
        {
            if (!IsSpawned)
            {
                return;
            }

            IsSpawned = false;
            OnObjectDespawn();
        }

        /// <summary>
        /// Wrapper for <see cref="OnObjectSpawn"/> that checks and toggles the <see cref="IsSpawned"/> flag.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal void Spawn()
        {
            if (IsSpawned)
            {
                return;
            }

            IsSpawned = true;
            OnObjectSpawn();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Destroy(bool callDestroy)
        {
            if (_isDestroyed)
            {
                return;
            }

            _isDestroyed = true;
            OnObjectDestroy();

            if (_isQuitting)
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

            Addressables.ReleaseInstance(CachedGameObject);

            if (callDestroy)
            {
                Object.Destroy(CachedGameObject);
            }

            OnActiveStateChanged = null;
            OnDestroyed = null;
            GameObjectUtility.RemoveCachedBehaviour(GameObjectID);
            CachedGameObject = null;
            CachedTransform = null;
            Pool = null;
        }
    }
}
