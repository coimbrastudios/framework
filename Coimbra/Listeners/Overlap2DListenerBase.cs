using System.Collections.Generic;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Listen to 2D overlaps.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class Overlap2DListenerBase<T> : MonoBehaviour
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
        [Min(0)]
        [Tooltip("Min interval to check for overlaps. This is a minimum value as it is also based on the player loop listener component update rate.")]
        private float _minInterval = 0.1f;

        [SerializeField]
        [Tooltip("The contact filter to use for overlaps.")]
        private ContactFilter2D _contactFilter;

        [SerializeField]
        [Disable]
        [Tooltip("The current list of overlaps.")]
        private List<Collider2D> _currentList = new List<Collider2D>();

        private HashSet<Collider2D> _currentSet = new HashSet<Collider2D>();

        private T _component;

        /// <summary>
        /// The current amount of overlaps.
        /// </summary>
        public int OverlapCount => _currentList.Count;

        /// <summary>
        /// The component this component depends on.
        /// </summary>
        public T Component => _component != null ? _component : _component = GetComponent<T>();

        /// <summary>
        /// The time when the last overlap check happened.
        /// </summary>
        public float LastOverlapTime { get; private set; }

        /// <summary>
        /// Min interval to check for overlaps. This is a minimum value as it is also based on the player loop listener component update rate.
        /// </summary>
        public float MinInterval
        {
            get => _minInterval;
            set => _minInterval = Mathf.Max(value, 0);
        }

        /// <summary>
        /// The contact filter to use for overlaps.
        /// </summary>
        public ContactFilter2D ContactFilter
        {
            get => _contactFilter;
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
        /// Override this method to define how the overlap should happens.
        /// </summary>
        protected abstract int Overlap(ref ContactFilter2D contactFilter, List<Collider2D> results);

        private void FixedUpdate()
        {
            if (LastOverlapTime + _minInterval > Time.time)
            {
                return;
            }

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
    }
}
