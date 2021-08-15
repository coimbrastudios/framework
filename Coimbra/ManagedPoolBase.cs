using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.Serialization;

namespace Coimbra
{
    /// <summary>
    /// Inherit from this class to create your own pools for managed objects.
    /// </summary>
    [Preserve]
    [Serializable]
    public abstract class ManagedPoolBase<T>
        where T : class
    {
        [FormerlySerializedAs("m_PreloadCount")]
        [Tooltip("Amount of items available from the beginning.")]
        [SerializeField]
        [Min(0)]
        private int _preloadCount;
        [FormerlySerializedAs("m_MaxCapacity")]
        [Tooltip("Max amount of items in the pool. If 0 it is treated as infinity capacity.")]
        [SerializeField]
        [Min(0)]
        private int _maxCapacity;

        private readonly object _lock = new object();
        private readonly Disposable<T>.DisposeHandler _disposeHandler;
        private readonly HashSet<T> _availableSet = new HashSet<T>();
        private readonly Stack<T> _availableStack = new Stack<T>();

        /// <summary>
        /// If not called on the inherited type the <see cref="GetDisposable"/> method won't work properly.
        /// </summary>
        protected ManagedPoolBase()
        {
            _disposeHandler = Release;
        }

        /// <summary>
        /// If not called on the inherited type the <see cref="GetDisposable"/> method won't work properly.
        /// </summary>
        protected ManagedPoolBase(int preloadCount, int maxCapacity)
            : this()
        {
            _preloadCount = Mathf.Max(preloadCount, 0);
            _maxCapacity = Mathf.Max(maxCapacity, 0);
        }

        /// <summary>
        /// Max amount of items in the pool. If 0 it is treated as infinity capacity.
        /// </summary>
        public int MaxCapacity
        {
            get => _maxCapacity;
            set => _maxCapacity = Mathf.Max(value, 0);
        }

        /// <summary>
        /// Amount of items available from the beginning.
        /// </summary>
        public int PreloadCount
        {
            get => _preloadCount;
            set => _preloadCount = Mathf.Max(value, 0);
        }

        /// <summary>
        /// Reset the pool to its initial state.
        /// </summary>
        /// <param name="preloadCount">If not null. it will override the current <see cref="PreloadCount"/>.</param>
        /// <param name="maxCapacity">If not null, it will override the current <see cref="MaxCapacity"/>.</param>
        public void Reset(int? preloadCount = null, int? maxCapacity = null)
        {
            lock (_lock)
            {
                if (preloadCount.HasValue)
                {
                    PreloadCount = preloadCount.Value;
                }

                if (maxCapacity.HasValue)
                {
                    MaxCapacity = maxCapacity.Value;
                }

                int desiredCount = MaxCapacity > 0 ? Mathf.Min(PreloadCount, MaxCapacity) : PreloadCount;
                bool preload = false;

                if (_availableStack.Count < desiredCount)
                {
                    preload = true;
                }
                else if (MaxCapacity != 0)
                {
                    while (_availableStack.Count > desiredCount)
                    {
                        Delete();
                    }
                }

                if (preload)
                {
                    Preload(desiredCount);
                }
            }
        }

        /// <summary>
        /// Pick one item from the pool.
        /// </summary>
        [NotNull]
        public T Get()
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

            item ??= OnCreate();

            OnGet(item);

            return item;
        }

        /// <summary>
        /// Pick one item from the pool using a <see cref="Disposable{T}"/>.
        /// </summary>
        public Disposable<T> GetDisposable()
        {
            T item = Get();

            return new Disposable<T>(item, _disposeHandler);
        }

        /// <summary>
        /// Return the item to the pool.
        /// </summary>
        public void Release([NotNull] in T item)
        {
            bool release = false;

            lock (_lock)
            {
                if (_availableSet.Contains(item))
                {
                    return;
                }

                if (MaxCapacity == 0 || _availableStack.Count < MaxCapacity)
                {
                    _availableStack.Push(item);
                    _availableSet.Add(item);
                    release = true;
                }
            }

            if (release)
            {
                OnRelease(item);
            }
            else
            {
                OnDelete(item);
            }
        }

        /// <summary>
        /// Called when creating a new item for the pool. It should never return null.
        /// </summary>
        [NotNull]
        protected abstract T OnCreate();

        /// <summary>
        /// Called when deleting an item due the pool being full.
        /// </summary>
        protected abstract void OnDelete([NotNull] T item);

        /// <summary>
        /// Called when picking an item from the pool.
        /// </summary>
        protected abstract void OnGet([NotNull] T item);

        /// <summary>
        /// Called when returning an item from the pool.
        /// </summary>
        protected abstract void OnRelease([NotNull] T item);

        /// <summary>
        /// Fill the pool with items.
        /// </summary>
        /// <param name="desiredCount">If null, it will pick the pool's <see cref="PreloadCount"/>.</param>
        protected void Preload(int? desiredCount = null)
        {
            lock (_lock)
            {
                if (desiredCount.HasValue == false)
                {
                    desiredCount = MaxCapacity > 0 ? Mathf.Min(PreloadCount, MaxCapacity) : PreloadCount;
                }

                while (_availableStack.Count < desiredCount)
                {
                    Create();
                }
            }
        }

        private void Create()
        {
            T item = OnCreate();
            _availableSet.Add(item);
            _availableStack.Push(item);
        }

        private void Delete()
        {
            T item = _availableStack.Pop();
            _availableSet.Remove(item);
            OnDelete(item);
        }
    }
}
