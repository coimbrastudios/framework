using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to 2D overlaps.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerLoopListenerBase))]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public abstract class Overlap2DListenerBase<T> : ActorComponentBase
        where T : Component
    {
        public delegate void OverlapEventHandler(Overlap2DListenerBase<T> sender, Collider2D other);

        /// <summary>
        /// Invoked when the overlap begins for a given collider.
        /// </summary>
        public event OverlapEventHandler OnBegin;

        /// <summary>
        /// Invoked when the overlap ends for a given collider.
        /// </summary>
        public event OverlapEventHandler OnEnd;

        [SerializeField]
        [DisableOnPlayMode]
        [Tooltip("If true, it will do an initial check during OnPostInitializeActor.")]
        private bool _checkOverlapsOnPostInitializeActor = true;

        [SerializeField]
        [Min(0)]
        [Tooltip("Min interval to check for overlaps. This is a minimum value as it is also based on the player loop listener component update rate.")]
        private float _minInterval = 0.1f;

        [SerializeField]
        [Tooltip("The contact filter to use for overlaps.")]
        private ContactFilter2D _contactFilter;

        [SerializeField]
        [Disable]
        [Tooltip("The current list of overlaps.")]
        private List<Collider2D> _currentList = new();

        private HashSet<Collider2D> _currentSet = new();

        private T _component;

        private PlayerLoopListenerBase _playerLoopListener;

        protected Overlap2DListenerBase()
        {
            _contactFilter.NoFilter();
        }

        /// <summary>
        /// Gets the current amount of overlaps.
        /// </summary>
        public int OverlapCount => _currentList.Count;

        /// <summary>
        /// Gets the time when the last overlap check happened.
        /// </summary>
        public float LastOverlapTime { get; private set; }

        /// <summary>
        /// Gets the component this component depends on.
        /// </summary>
        public T Component => _component != null ? _component : _component = GetComponent<T>();

        /// <summary>
        /// Gets the player loop listener this component depends on.
        /// </summary>
        public PlayerLoopListenerBase PlayerLoopListener => _playerLoopListener != null ? _playerLoopListener : _playerLoopListener = GetComponent<PlayerLoopListenerBase>();

        /// <summary>
        /// Gets or sets a value indicating whether it should do an initial check during <see cref="OnPostInitializeActor"/>.
        /// </summary>
        public bool CheckOverlapsOnPostInitializeActor
        {
            [DebuggerStepThrough]
            get => _checkOverlapsOnPostInitializeActor;
            [DebuggerStepThrough]
            set => _checkOverlapsOnPostInitializeActor = value;
        }

        /// <summary>
        /// Gets or sets min interval to check for overlaps. This is a minimum value as it is also based on the player loop listener component update rate.
        /// </summary>
        public float MinInterval
        {
            [DebuggerStepThrough]
            get => _minInterval;
            [DebuggerStepThrough]
            set => _minInterval = Mathf.Max(value, 0);
        }

        /// <summary>
        /// Gets or sets the contact filter to use for overlaps.
        /// </summary>
        public ContactFilter2D ContactFilter
        {
            [DebuggerStepThrough]
            get => _contactFilter;
            [DebuggerStepThrough]
            set => _contactFilter = value;
        }

        /// <summary>
        /// Clears the overlaps and the last overlap time.
        /// </summary>
        public void Clear()
        {
            LastOverlapTime = 0;
            _currentList.Clear();
            _currentSet.Clear();
        }

        /// <summary>
        /// Get overlap at the given index. The index shouldn't be greater than the <see cref="OverlapCount"/>.
        /// </summary>
        public Collider2D GetOverlap(int index)
        {
            return _currentList[index];
        }

        /// <summary>
        /// Checks if currently overlapping the given collider.
        /// </summary>
        public bool IsOverlapping(Collider2D other)
        {
            return _currentSet.Contains(other);
        }

        /// <summary>
        /// Force update the current overlaps.
        /// </summary>
        public void ForceUpdateOverlaps()
        {
            LastOverlapTime = Time.time;

            HashSet<Collider2D> previousSet = _currentSet;
            List<Collider2D> previousList = _currentList;
            _currentSet = HashSetPool.Pop<Collider2D>();
            _currentList = ListPool.Pop<Collider2D>();

            int count = Overlap(ref _contactFilter, _currentList);

            for (int i = 0; i < count; i++)
            {
                if (!previousSet.Contains(_currentList[i]))
                {
                    OnBegin?.Invoke(this, _currentList[i]);
                }
            }

            foreach (Collider2D other in previousList)
            {
                if (!_currentSet.Contains(other))
                {
                    OnEnd?.Invoke(this, other);
                }
            }

            HashSetPool.Push(previousSet);
            ListPool.Push(previousList);
        }

        /// <summary>
        /// Override this method to define how the overlap should happens.
        /// </summary>
        protected abstract int Overlap(ref ContactFilter2D contactFilter, List<Collider2D> results);

        /// <inheritdoc/>
        protected sealed override void OnPreInitializeActor()
        {
            PlayerLoopListener.OnTrigger += HandlePlayerLoop;
        }

        /// <inheritdoc/>
        protected sealed override void OnPostInitializeActor()
        {
            if (_checkOverlapsOnPostInitializeActor)
            {
                ForceUpdateOverlaps();
            }
        }

        /// <summary>
        /// Unity callback.
        /// </summary>
        protected void OnDestroy()
        {
            PlayerLoopListener.OnTrigger -= HandlePlayerLoop;
        }

        private void HandlePlayerLoop(PlayerLoopListenerBase sender, float deltaTime)
        {
            if (Time.time >= LastOverlapTime + _minInterval)
            {
                ForceUpdateOverlaps();
            }
        }
    }
}
