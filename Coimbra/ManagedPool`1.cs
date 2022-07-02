using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Stack-based tread-safe pool for any managed object. For pooling <see cref="GameObject"/> consider using <see cref="GameObjectPool"/>.
    /// </summary>
    [Preserve]
    [Serializable]
    public sealed class ManagedPool<T> : IManagedPool
        where T : class
    {
        /// <summary>
        /// Disposable pattern to use with <see cref="ManagedPool{T}"/> instances.
        /// </summary>
        [Preserve]
        public ref struct Instance
        {
            [CanBeNull]
            public readonly T Value;

            [NotNull]
            public readonly ManagedPool<T> Pool;

            private bool _isDisposed;

            /// <summary>
            /// Initializes a new instance of the <see cref="Instance"/> struct.
            /// </summary>
            public Instance([CanBeNull] in T value, [NotNull] ManagedPool<T> pool)
            {
                Value = value;
                Pool = pool;
                _isDisposed = false;
            }

            public void Dispose()
            {
                if (_isDisposed)
                {
                    return;
                }

                _isDisposed = true;
                Pool.Push(Value);
            }
        }

        /// <summary>
        /// Used for executing actions on an instance of the pool. It will always be a valid instance.
        /// </summary>
        public delegate void ActionHandler([NotNull] T instance);

        /// <summary>
        /// Used for creating a new instance for the pool. It should never return null.
        /// </summary>
        [NotNull]
        public delegate T CreateHandler();

        /// <summary>
        /// Called when deleting an instance due the pool being full. This will get called before actually disposing the instance.
        /// </summary>
        public event ActionHandler OnDelete;

        /// <summary>
        /// Called when picking an instance from the pool. This will be called even if the instance was just created.
        /// </summary>
        public event ActionHandler OnPop;

        /// <summary>
        /// Called when returning an instance to the pool. This will be called even if the instance is about to be deleted.
        /// </summary>
        public event ActionHandler OnPush;

        private readonly object _lock = new();

        private readonly HashSet<T> _availableSet = new();

        private readonly Stack<T> _availableStack = new();

        private readonly ActionHandler _disposeCallback;

        private readonly CreateHandler _createCallback;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedPool{T}"/> class.
        /// </summary>
        /// <param name="createCallback">Called when creating a new item for the pool. It should never return null.</param>
        /// <param name="disposeCallback">Called after deleting an item from the pool. This can be used to dispose any native resources.</param>
        public ManagedPool([NotNull] CreateHandler createCallback, [CanBeNull] ActionHandler disposeCallback = null)
        {
            _createCallback = createCallback;
            _disposeCallback = disposeCallback;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedPool{T}"/> class.
        /// </summary>
        /// <param name="createCallback">Called when creating a new instance for the pool. It should never return null.</param>
        /// <param name="preloadCount">Amount of instances available from the beginning.</param>
        /// <param name="maxCapacity">Max amount of instances in the pool. If 0 it is treated as infinity capacity.</param>
        public ManagedPool([NotNull] CreateHandler createCallback, int preloadCount, int maxCapacity)
            : this(createCallback)
        {
            Initialize(preloadCount, maxCapacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedPool{T}"/> class.
        /// </summary>
        /// <param name="createCallback">Called when creating a new instance for the pool. It should never return null.</param>
        /// <param name="disposeCallback">Called after deleting an item from the pool. This can be used to dispose any native resources.</param>
        /// <param name="preloadCount">Amount of instances available from the beginning.</param>
        /// <param name="maxCapacity">Max amount of instances in the pool. If 0 it is treated as infinity capacity.</param>
        public ManagedPool([NotNull] CreateHandler createCallback, [CanBeNull] ActionHandler disposeCallback, int preloadCount, int maxCapacity)
            : this(createCallback, disposeCallback)
        {
            Initialize(preloadCount, maxCapacity);
        }

        /// <summary>
        /// Gets the current amount of instances available.
        /// </summary>
        [field: SerializeField]
        [field: Disable]
        [field: Tooltip("The current amount of instances available.")]
        public int AvailableCount { get; private set; }

        /// <summary>
        /// Gets max amount of instances in the pool. If 0 it is treated as infinity capacity.
        /// </summary>
        [field: SerializeField]
        [field: Disable]
        [field: Tooltip("Max amount of instances in the pool. If 0 it is treated as infinity capacity.")]
        public int MaxCapacity { get; private set; }

        /// <summary>
        /// Gets amount of instances available from the beginning.
        /// </summary>
        [field: SerializeField]
        [field: Disable]
        [field: Tooltip("Amount of instances available from the beginning.")]
        public int PreloadCount { get; private set; }

        /// <summary>
        /// Should only be used from inside a type with <see cref="SharedManagedPoolAttribute"/>.
        /// </summary>
        /// <param name="createCallback">Called when creating a new instance for the pool. It should never return null.</param>
        /// <param name="disposeCallback">Called after deleting an item from the pool. This can be used to dispose any native resources.</param>
        public static ManagedPool<T> CreateShared([NotNull] CreateHandler createCallback, [CanBeNull] ActionHandler disposeCallback = null)
        {
            ManagedPool<T> managedPool = new(createCallback, disposeCallback);
            SharedManagedPoolUtility.All.Add(new WeakReference<IManagedPool>(managedPool));

            return managedPool;
        }

        /// <summary>
        /// Should only be used from inside a type with <see cref="SharedManagedPoolAttribute"/>.
        /// </summary>
        /// <param name="createCallback">Called when creating a new instance for the pool. It should never return null.</param>
        /// <param name="preloadCount">Amount of instances available from the beginning.</param>
        /// <param name="maxCapacity">Max amount of instances in the pool. If 0 it is treated as infinity capacity.</param>
        public static ManagedPool<T> CreateShared([NotNull] CreateHandler createCallback, int preloadCount, int maxCapacity)
        {
            ManagedPool<T> managedPool = new(createCallback, preloadCount, maxCapacity);
            SharedManagedPoolUtility.All.Add(new WeakReference<IManagedPool>(managedPool));

            return managedPool;
        }

        /// <summary>
        /// Should only be used from inside a type with <see cref="SharedManagedPoolAttribute"/>.
        /// </summary>
        /// <param name="createCallback">Called when creating a new instance for the pool. It should never return null.</param>
        /// <param name="disposeCallback">Called after deleting an item from the pool. This can be used to dispose any native resources.</param>
        /// <param name="preloadCount">Amount of instances available from the beginning.</param>
        /// <param name="maxCapacity">Max amount of instances in the pool. If 0 it is treated as infinity capacity.</param>
        public static ManagedPool<T> CreateShared([NotNull] CreateHandler createCallback, [CanBeNull] ActionHandler disposeCallback, int preloadCount, int maxCapacity)
        {
            ManagedPool<T> managedPool = new(createCallback, disposeCallback, preloadCount, maxCapacity);
            SharedManagedPoolUtility.All.Add(new WeakReference<IManagedPool>(managedPool));

            return managedPool;
        }

        /// <summary>
        /// Initializes the pool, returning it to its initial state if already being used.
        /// </summary>
        /// <param name="preloadCount">If not null. it will override the current <see cref="PreloadCount"/>.</param>
        /// <param name="maxCapacity">If not null, it will override the current <see cref="MaxCapacity"/>.</param>
        public void Initialize(int? preloadCount = null, int? maxCapacity = null)
        {
            lock (_lock)
            {
                if (preloadCount.HasValue)
                {
                    PreloadCount = Mathf.Max(preloadCount.Value, 0);
                }

                if (maxCapacity.HasValue)
                {
                    MaxCapacity = Mathf.Max(maxCapacity.Value, 0);
                }

                AvailableCount = MaxCapacity > 0 ? Mathf.Min(PreloadCount, MaxCapacity) : PreloadCount;

                if (_availableStack.Count < AvailableCount)
                {
                    do
                    {
                        T instance = _createCallback();
                        _availableSet.Add(instance);
                        _availableStack.Push(instance);
                    }
                    while (_availableStack.Count < AvailableCount);
                }
                else
                {
                    while (_availableStack.Count > AvailableCount)
                    {
                        T instance = _availableStack.Pop();
                        _availableSet.Remove(instance);
                        OnDelete?.Invoke(instance);
                        _disposeCallback?.Invoke(instance);
                    }
                }
            }
        }

        /// <summary>
        /// Picks one instance from the pool.
        /// </summary>
        [NotNull]
        public T Pop()
        {
            T item = null;

            lock (_lock)
            {
                if (_availableStack.Count > 0)
                {
                    item = _availableStack.Pop();
                    _availableSet.Remove(item);
                    AvailableCount--;
                }
            }

            item ??= _createCallback();
            OnPop?.Invoke(item);

            return item;
        }

        /// <summary>
        /// Picks one instance from the pool as a <see cref="Instance"/>.
        /// </summary>
        public Instance Pop([NotNull] out T instance)
        {
            instance = Pop();

            return new Instance(instance, this);
        }

        /// <summary>
        /// Returns the instance to the pool.
        /// </summary>
        public void Push([NotNull] in T instance)
        {
            bool dispose = true;

            lock (_lock)
            {
                if (_availableSet.Contains(instance))
                {
                    return;
                }

                if (MaxCapacity == 0 || _availableStack.Count < MaxCapacity)
                {
                    AvailableCount++;
                    _availableStack.Push(instance);
                    _availableSet.Add(instance);
                    OnPush?.Invoke(instance);
                    dispose = false;
                }
            }

            if (!dispose)
            {
                return;
            }

            OnDelete?.Invoke(instance);
            _disposeCallback?.Invoke(instance);
        }
    }
}
