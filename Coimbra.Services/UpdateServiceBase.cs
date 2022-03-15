using System.Collections.Generic;
using UnityEngine;

namespace Coimbra.Services
{
    /// <summary>
    /// Base implementation of the default implementations of update-based services.
    /// </summary>
    [AddComponentMenu("")]
    public abstract class UpdateServiceBase<T> : MonoBehaviour
        where T : class
    {
        private readonly HashSet<T> _listenersSet = new HashSet<T>();
        private readonly List<T> _listenersList = new List<T>();

        protected IReadOnlyList<T> Listeners => _listenersList;

        /// <summary>
        /// Add a listener to this service.
        /// </summary>
        public void AddListener(T listener)
        {
            if (_listenersSet.Add(listener))
            {
                _listenersList.Add(listener);
            }
        }

        /// <summary>
        /// Add a listener to this service.
        /// </summary>
        public void ClearListeners()
        {
            _listenersSet.Clear();
            _listenersList.Clear();
        }

        /// <summary>
        /// Remove a listener from this service.
        /// </summary>
        public void RemoveListener(T listener)
        {
            if (_listenersSet.Remove(listener))
            {
                _listenersList.Remove(listener);
            }
        }
    }
}
