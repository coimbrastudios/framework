using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using Object = UnityEngine.Object;

namespace Coimbra
{
    /// <summary>
    /// Stack-based tread-safe pool for any managed object. For pooling <see cref="GameObject"/> consider using <see cref="GameObjectPool"/>.
    /// </summary>
    [Preserve]
    public sealed class ManagedPool<T>
        where T : class
    {
        /// <summary>
        /// Disposable pattern to use with <see cref="ManagedPool{T}"/> instances.
        /// </summary>
        [Preserve]
        public readonly ref struct Instance
        {
            [CanBeNull]
            public readonly T Value;

            [NotNull]
            public readonly ManagedPool<T> Pool;

            public Instance([CanBeNull] in T value, [NotNull] ManagedPool<T> pool)
            {
                Value = value;
                Pool = pool;
            }

            public void Dispose()
            {
                Pool.Push(Value);
            }
        }

        /// <summary>
        /// Used for creating a new instance for the pool. It should never return null.
        /// </summary>
        [NotNull]
        public delegate T CreateHandler();

        /// <summary>
        /// Called when deleting an instance due the pool being full.
        /// </summary>
        public event Action<T> OnDelete;

        /// <summary>
        /// Called when picking an instance from the pool.
        /// </summary>
        public event Action<T> OnPop;

        /// <summary>
        /// Called when returning an instance to the pool.
        /// </summary>
        public event Action<T> OnPush;

        private readonly object _lock = new object();

        private readonly HashSet<T> _availableSet = new HashSet<T>();

        private readonly Stack<T> _availableStack = new Stack<T>();

        private readonly CreateHandler _createCallback;

        private readonly Action<T> _disposeCallback;

        /// <param name="createCallback">Called when creating a new item for the pool. It should never return null.</param>
        /// <param name="disposeCallback">Called after deleting an item from the pool. This can be used to dispose from native resources.</param>
        public ManagedPool([NotNull] CreateHandler createCallback, [CanBeNull] Action<T> disposeCallback = null)
        {
            _createCallback = createCallback;
            _disposeCallback = disposeCallback;
        }

        /// <param name="createCallback">Called when creating a new instance for the pool. It should never return null.</param>
        /// <param name="preloadCount">Amount of instances available from the beginning.</param>
        /// <param name="maxCapacity">Max amount of instances in the pool. If 0 it is treated as infinity capacity.</param>
        public ManagedPool([NotNull] CreateHandler createCallback, int preloadCount, int maxCapacity)
            : this(createCallback)
        {
            Initialize(preloadCount, maxCapacity);
        }

        /// <param name="createCallback">Called when creating a new instance for the pool. It should never return null.</param>
        /// <param name="disposeCallback">Called after deleting an item from the pool. This can be used to dispose from native resources.</param>
        /// <param name="preloadCount">Amount of instances available from the beginning.</param>
        /// <param name="maxCapacity">Max amount of instances in the pool. If 0 it is treated as infinity capacity.</param>
        public ManagedPool([NotNull] CreateHandler createCallback, [CanBeNull] Action<T> disposeCallback, int preloadCount, int maxCapacity)
            : this(createCallback, disposeCallback)
        {
            Initialize(preloadCount, maxCapacity);
        }

        /// <summary>
        /// Max amount of instances in the pool. If 0 it is treated as infinity capacity.
        /// </summary>
        public int MaxCapacity { get; private set; }

        /// <summary>
        /// Amount of instances available from the beginning.
        /// </summary>
        public int PreloadCount { get; private set; }

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

                int targetCount = MaxCapacity > 0 ? Mathf.Min(PreloadCount, MaxCapacity) : PreloadCount;

                if (_availableStack.Count < targetCount)
                {
                    do
                    {
                        T instance = _createCallback();
                        _availableSet.Add(instance);
                        _availableStack.Push(instance);
                    }
                    while (_availableStack.Count < targetCount);
                }
                else
                {
                    while (_availableStack.Count > targetCount)
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

    /// <summary>
    /// Static implementation of <see cref="ManagedPool{T}"/> for objects with a default constructor. It also has special treatment <see cref="IDisposable"/> and <see cref="Object"/> types to correctly delete those.
    /// </summary>
    [Preserve]
    [SharedManagedPool("Value", "Instance")]
    public static partial class ManagedPool
    {
        [Preserve]
        private static class Instance<T>
            where T : class, new()
        {
            internal static readonly ManagedPool<T> Value;

            static Instance()
            {
                static T createCallback()
                {
                    return new T();
                }

                Action<T> disposeCallback = null;

                if (typeof(IDisposable).IsAssignableFrom(typeof(T)))
                {
                    disposeCallback += delegate(T obj)
                    {
                        if (obj.TryGetValid(out T valid))
                        {
                            ((IDisposable)valid).Dispose();
                        }
                    };
                }

                if (typeof(Object).IsAssignableFrom(typeof(T)))
                {
                    disposeCallback += delegate(T obj)
                    {
                        (obj as Object).Destroy();
                    };
                }

                Value = new ManagedPool<T>(createCallback, disposeCallback);
            }
        }
    }
}
