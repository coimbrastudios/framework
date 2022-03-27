using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace Coimbra
{
    [PublicAPI]
    [Preserve]
    [AddComponentMenu(FrameworkUtility.AddComponentMenuPath + "GameObject Pool")]
    public sealed class GameObjectPool : MonoBehaviour
    {
        /// <summary>
        /// Results available when trying to despawn an object.
        /// </summary>
        public enum DespawnResult
        {
            /// <summary>
            /// Successfully despawned the object.
            /// </summary>
            Despawned,
            /// <summary>
            /// The object got destroyed.
            /// </summary>
            Destroyed,
            /// <summary>
            /// Aborted the operation because the object does not belong to the pool.
            /// </summary>
            Aborted
        }

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
            /// <summary>
            /// Pool is unloading its instances.
            /// </summary>
            Unloading,
        }

        /// <summary>
        /// Delegate for listening when an object instantiates on a pool.
        /// </summary>
        public delegate void ObjectInstantiateHandler(GameObjectPool pool, GameObjectBehaviour instance);

        /// <summary>
        /// Delegate for listening state changes of a pool.
        /// </summary>
        public delegate void StateChangeHandler(GameObjectPool pool, State previous, State current);

        /// <summary>
        /// Invoked when an object is instantiated.
        /// </summary>
        public event ObjectInstantiateHandler OnObjectInstantiated;

        /// <summary>
        /// Invoked when this pool <see cref="CurrentState"/> changes.
        /// </summary>
        public event StateChangeHandler OnStateChanged;

        [SerializeField]
        [Tooltip("The current pool state.")]
        private State _currentState;
        [SerializeField]
        [Tooltip("The prefab that this pool is using.")]
        private GameObjectBehaviour _prefab;
        [SerializeField]
        [Tooltip("If true, instantiate will be used when spawn fails.")]
        private bool _canInstantiateOnSpawn = true;
        [SerializeField]
        [Tooltip("If true, new instances will receive a more descriptive name. (Editor Only)")]
        private bool _changeNameOnInstantiate = true;
        [SerializeField]
        [Tooltip("If true, parent will not change automatically when despawned. Changing to false affects performance.")]
        private bool _keepParentOnDespawn = true;
        [SerializeField]
        [Tooltip("Amount of instances available from the beginning.")]
        [Min(0)]
        private int _preloadCount = 1;
        [SerializeField]
        [Tooltip("Max amount of instances in the pool. If 0 it is treated as infinity capacity.")]
        [Min(0)]
        private int _maxCapacity = 1;
        [SerializeField]
        [Tooltip("The transform to be used as the container.")]
        private Transform _containerTransform;

        private HashSet<GameObjectID> _availableInstancesIds;
        private Stack<GameObjectBehaviour> _availableInstances;

        /// <summary>
        /// The current pool state.
        /// </summary>
        public State CurrentState => _currentState;

        /// <summary>
        /// If true, instantiate will be used when spawn fails.
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
        /// If true, parent will not change automatically when despawned. Changing to false affects performance.
        /// </summary>
        public bool KeepParentOnDespawn
        {
            get => _keepParentOnDespawn;
            set => _keepParentOnDespawn = value;
        }

        /// <summary>
        /// Amount of instances available from the beginning.
        /// </summary>
        public int PreloadCount
        {
            get => _preloadCount;
            set => _preloadCount = Mathf.Max(0, value);
        }

        /// <summary>
        /// Max amount of instances in the pool. If 0 it is treated as infinity capacity.
        /// </summary>
        public int MaxCapacity
        {
            get => _maxCapacity;
            set
            {
                _maxCapacity = Mathf.Max(0, value);

                if (_maxCapacity == 0)
                {
                    return;
                }

                while (_availableInstances.Count > _maxCapacity)
                {
                    GameObjectBehaviour instance = _availableInstances.Pop();

                    if (instance == null)
                    {
                        continue;
                    }

                    _availableInstancesIds.Remove(instance);
                    instance.Destroy();
                }
            }
        }

        /// <summary>
        /// The prefab that this pool is using.
        /// </summary>
        public GameObjectBehaviour Prefab
        {
            get => _prefab;
            set
            {
                if (_currentState == State.Loading || _currentState == State.Loaded)
                {
                    Debug.LogError($"Can't change the prefab of a {nameof(GameObjectPool)} currently in use!", gameObject);
                }
                else
                {
                    _prefab = value;
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

                foreach (GameObjectBehaviour instance in _availableInstances)
                {
                    instance.CachedTransform.SetParent(_containerTransform, false);
                }
            }
        }

        /// <summary>
        /// Despawns the specified instance.
        /// </summary>
        /// <param name="instance">The instance to despawn.</param>
        /// <returns>The result of the call.</returns>
        public DespawnResult Despawn(GameObject instance)
        {
            return Despawn(instance.GetOrCreateBehaviour());
        }

        /// <inheritdoc cref="GameObjectPool.Despawn(UnityEngine.GameObject)"/>
        public DespawnResult Despawn(GameObjectBehaviour instance)
        {
            if (_currentState == State.Unloaded)
            {
                GameObject o = gameObject;
                Debug.LogWarning($"{o} will use destroy instead of despawn for all objects because it is unloaded!", o);
                instance.Destroy();

                return DespawnResult.Destroyed;
            }

            GameObjectID id = instance;

            if (_availableInstancesIds.Contains(id))
            {
                return DespawnResult.Aborted;
            }

            instance.Despawn();

            if (_maxCapacity == 0 || _availableInstances.Count < _maxCapacity)
            {
                if (!_keepParentOnDespawn)
                {
                    instance.transform.SetParent(_containerTransform, false);
                }

                _availableInstances.Push(instance);
                _availableInstancesIds.Add(id);

                return DespawnResult.Despawned;
            }

            instance.Destroy();

            return DespawnResult.Destroyed;
        }

        public void Unload()
        {
            for (int i = 0,
                     count = _availableInstances.Count;
                 i < count;
                 i++)
            {
                _availableInstances.Pop().GetValid()?.Destroy();
            }

            _availableInstances = null;
            _availableInstancesIds = null;
            OnStateChanged?.Invoke(this, State.Loaded, State.Unloaded);
        }

        public void Load()
        {
            if (_prefab == null)
            {
                GameObject o = gameObject;
                Debug.LogError($"{o} requires a non-null prefab to load!", o);

                return;
            }

            int instantiateCount = _maxCapacity == 0 ? _preloadCount : Mathf.Min(_preloadCount, _maxCapacity);
            _availableInstances = new Stack<GameObjectBehaviour>(instantiateCount);
            _availableInstancesIds = new HashSet<GameObjectID>();

            for (int i = 0; i < instantiateCount; i++)
            {
                GameObjectBehaviour instance = Instantiate(_containerTransform, false);
                instance.CachedGameObject.SetActive(false);
                _availableInstances.Push(instance);
                _availableInstancesIds.Add(instance);
            }

            OnStateChanged?.Invoke(this, State.Unloaded, State.Loaded);
        }

        /// <summary>
        /// Spawns the an instance with the specified transform.
        /// </summary>
        /// <param name="parent">The instance parent.</param>
        /// <param name="spawnInWorldSpace">If false, the instance transform will be relative to the specified parent.</param>
        /// <returns>The spawned instance.</returns>
        public GameObjectBehaviour Spawn(Transform parent = null, bool spawnInWorldSpace = false)
        {
            if (_currentState == State.Unloaded)
            {
                GameObject o = gameObject;

                if (_canInstantiateOnSpawn)
                {
                    Debug.LogWarning($"{o} will use instantiate instead of a valid instance while unloaded!", o);

                    return Instantiate(parent, spawnInWorldSpace);
                }

                Debug.LogWarning($"{o} will return null instead of a valid instance while unloaded!", o);

                return null;
            }

            while (_availableInstances.Count > 0)
            {
                GameObjectBehaviour instance = _availableInstances.Pop();

                if (instance == null)
                {
                    continue;
                }

                _availableInstancesIds.Remove(instance);
                instance.CachedTransform.SetParent(parent, spawnInWorldSpace);
                instance.Spawn();

                return instance;
            }

            if (_maxCapacity == 0 || _availableInstances.Count < _maxCapacity || _canInstantiateOnSpawn)
            {
                GameObjectBehaviour instance = Instantiate(parent, spawnInWorldSpace);
                instance.Spawn();

                return instance;
            }

            return null;
        }

        /// <summary>
        /// Spawns the an instance with the specified transform.
        /// </summary>
        /// <param name="position">The position in world space.</param>
        /// <param name="rotation">The rotation in world space..</param>
        /// <param name="parent">The instance parent.</param>
        /// <returns>The spawned instance.</returns>
        public GameObjectBehaviour Spawn(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (_currentState == State.Unloaded)
            {
                GameObject o = gameObject;

                if (_canInstantiateOnSpawn)
                {
                    Debug.LogWarning($"{o} will use instantiate instead of a valid instance while unloaded!", o);

                    return Instantiate(position, rotation, parent);
                }

                Debug.LogWarning($"{o} will return null instead of a valid instance while unloaded!", o);

                return null;
            }

            while (_availableInstances.Count > 0)
            {
                GameObjectBehaviour instance = _availableInstances.Pop();

                if (instance == null)
                {
                    continue;
                }

                _availableInstancesIds.Remove(instance);
                instance.CachedTransform.parent = parent;
                instance.CachedTransform.SetPositionAndRotation(position, rotation);
                instance.Spawn();

                return instance;
            }

            if (_maxCapacity == 0 || _availableInstances.Count < _maxCapacity || _canInstantiateOnSpawn)
            {
                GameObjectBehaviour instance = Instantiate(position, rotation, parent);
                instance.Spawn();

                return instance;
            }

            return null;
        }

        /// <inheritdoc cref="GameObjectPool.Spawn(Vector3, Quaternion, Transform)"/>
        public GameObjectBehaviour Spawn(Vector3 position, Vector3 rotation, Transform parent = null)
        {
            return Spawn(position, Quaternion.Euler(rotation), parent);
        }

        /// <inheritdoc cref="GameObjectPool.Spawn(Transform, bool)"/>
        public T Spawn<T>(Transform parent = null, bool spawnInWorldSpace = false)
            where T : GameObjectBehaviour
        {
            return Spawn(parent, spawnInWorldSpace) as T;
        }

        /// <inheritdoc cref="GameObjectPool.Spawn(Vector3, Quaternion, Transform)"/>
        public T Spawn<T>(Vector3 position, Quaternion rotation, Transform parent = null)
            where T : GameObjectBehaviour
        {
            return Spawn(position, rotation, parent) as T;
        }

        /// <inheritdoc cref="GameObjectPool.Spawn(Vector3, Quaternion, Transform)"/>
        public T Spawn<T>(Vector3 position, Vector3 rotation, Transform parent = null)
            where T : GameObjectBehaviour
        {
            return Spawn(position, Quaternion.Euler(rotation), parent) as T;
        }

        private void Reset()
        {
            _containerTransform = transform;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Instantiate(GameObjectBehaviour instance)
        {
#if UNITY_EDITOR
            if (_changeNameOnInstantiate)
            {
                instance.CachedGameObject.name = $"{_prefab.name} ({Guid.NewGuid()})";
            }
#endif
            instance.Instantiate();
            OnObjectInstantiated?.Invoke(this, instance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private GameObjectBehaviour Instantiate(Transform parent, bool instantiateInWorldSpace)
        {
            GameObjectBehaviour instance = Instantiate(_prefab, parent, instantiateInWorldSpace);
            Instantiate(instance);

            return instance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private GameObjectBehaviour Instantiate(Vector3 position, Quaternion rotation, Transform parent)
        {
            GameObjectBehaviour instance = Instantiate(_prefab, position, rotation, parent);
            Instantiate(instance);

            return instance;
        }
    }
}
