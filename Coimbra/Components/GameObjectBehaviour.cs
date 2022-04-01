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

        private bool _isAwaken;
        private bool _isDestroyed;
        private bool _isInitialized;
        private bool _isQuitting;

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
        [field: SerializeField]
        [field: Disable]
        public bool IsPooled { get; private set; }

        /// <summary>
        /// Indicates if the object is currently spawned or should be treated as non-spawned.
        /// </summary>
        [field: SerializeField]
        [field: Disable]
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
            if (_isDestroyed)
            {
                return;
            }

            _isDestroyed = true;
            Despawn();
            OnObjectDestroy();

            if (!_isAwaken)
            {
                OnDestroyed?.Invoke(this, DestroyReason.ExplicitCall);
                GameObjectUtility.RemoveCachedBehaviour(CachedGameObject);
            }

            if (!Addressables.ReleaseInstance(CachedGameObject))
            {
                Destroy(CachedGameObject);
            }

            CachedGameObject = null;
            CachedTransform = null;
            Pool = null;
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
            GameObjectUtility.AddCachedBehaviour(this);
            OnObjectInitialize();
        }

        /// <summary>
        /// Non-virtual by design, use <see cref="OnObjectInitialize"/> instead.
        /// </summary>
        protected void Awake()
        {
            _isAwaken = true;
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
            _isDestroyed = true;
            CachedGameObject = gameObject;
            CachedTransform = transform;

            if (!IsPooled)
            {
                OnObjectDestroy();
            }

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

            OnActiveStateChanged = null;
            OnDestroyed = null;
            GameObjectUtility.RemoveCachedBehaviour(CachedGameObject);
        }

        /// <summary>
        /// Non-virtual by design, use <see cref="ApplicationQuitEvent"/> instead.
        /// </summary>
        protected void OnApplicationQuit()
        {
            _isQuitting = true;
        }

        /// <summary>
        /// Called each time this object is despawned. Will be called even if the object was inactive already, but will be called before OnDisable if was active.
        /// </summary>
        protected virtual void OnObjectDespawn()
        {
            CachedGameObject.SetActive(false);
        }

        /// <summary>
        /// Use this for one-time un-initializations instead of OnDestroy callback. This method is called even if the object starts inactive.
        /// </summary>
        protected virtual void OnObjectDestroy() { }

        /// <summary>
        /// Use this for one-time initializations instead of Awake or Start callbacks. This method is called even if the object starts inactive.
        /// </summary>
        protected virtual void OnObjectInitialize() { }

        /// <summary>
        /// Called each time this object is spawned. Will be called even if the object was active already, but will be called after OnEnable if was inactive.
        /// </summary>
        protected virtual void OnObjectSpawn()
        {
            CachedGameObject.SetActive(true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool Despawn()
        {
            if (!IsSpawned)
            {
                return false;
            }

            IsSpawned = false;
            OnObjectDespawn();

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool Spawn()
        {
            if (IsSpawned)
            {
                return false;
            }

            Initialize();
            IsSpawned = true;
            OnObjectSpawn();

            return true;
        }
    }
}
