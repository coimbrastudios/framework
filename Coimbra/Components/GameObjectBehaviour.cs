using JetBrains.Annotations;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace Coimbra
{
    [PublicAPI]
    [Preserve]
    [DisallowMultipleComponent]
    [AddComponentMenu(FrameworkUtility.AddComponentMenuPath + "GameObject Behaviour")]
    public class GameObjectBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Delegate for handling a <see cref="GameObject"/> active state changes.
        /// </summary>
        public delegate void ActiveStateHandler(GameObject sender, bool state);

        /// <summary>
        /// Delegate for handling a <see cref="GameObject"/> destroy.
        /// </summary>
        public delegate void DestroyHandler(GameObject sender, DestroyReason reason);

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
        private bool _isInstantiated;
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
        /// Non-virtual by design, use <see cref="OnObjectInstantiate"/> instead.
        /// </summary>
        protected void Awake()
        {
            _isAwaken = true;
            Instantiate();
        }

        /// <summary>
        /// Non-virtual by design, use <see cref="OnActiveStateChanged"/> instead.
        /// </summary>
        protected void OnEnable()
        {
            OnActiveStateChanged?.Invoke(CachedGameObject, true);
        }

        /// <summary>
        /// Non-virtual by design, use <see cref="OnActiveStateChanged"/> instead.
        /// </summary>
        protected void OnDisable()
        {
            OnActiveStateChanged?.Invoke(CachedGameObject, false);
        }

        /// <summary>
        /// Non-virtual by design, use <see cref="OnObjectDestroy"/> instead.
        /// </summary>
        protected void OnDestroy()
        {
            _isDestroyed = true;

            if (!IsPooled)
            {
                OnObjectDestroy();
            }

            if (_isQuitting)
            {
                OnDestroyed?.Invoke(CachedGameObject, DestroyReason.ApplicationQuit);
            }
            else if (CachedGameObject.scene.isLoaded)
            {
                OnDestroyed?.Invoke(CachedGameObject, DestroyReason.ExplicitCall);
            }
            else
            {
                OnDestroyed?.Invoke(CachedGameObject, DestroyReason.SceneChange);
            }

            OnActiveStateChanged = null;
            OnDestroyed = null;
            CachedGameObject.RemoveCachedBehaviour();
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
        protected virtual void OnObjectInstantiate() { }

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
        internal void Destroy()
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
                OnDestroyed?.Invoke(CachedGameObject, DestroyReason.ExplicitCall);
                CachedGameObject.RemoveCachedBehaviour();
            }

            Destroy(CachedGameObject);
            CachedGameObject = null;
            CachedTransform = null;
            Pool = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Instantiate()
        {
            if (_isInstantiated)
            {
                return;
            }

            _isInstantiated = true;
            IsPooled = Pool != null;
            CachedGameObject = gameObject;
            CachedTransform = transform;
            CachedGameObject.AddCachedBehaviour(this);
            OnObjectInstantiate();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool Spawn()
        {
            if (IsSpawned)
            {
                return false;
            }

            Instantiate();
            IsSpawned = true;
            OnObjectSpawn();

            return true;
        }
    }
}
