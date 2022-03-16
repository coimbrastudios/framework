using System.Collections.Generic;
using UnityEngine;

namespace Coimbra.Services
{
    /// <summary>
    /// Base implementation of the default implementations of update-based services.
    /// </summary>
    [AddComponentMenu("")]
    public abstract class UpdateSystemBase<T> : MonoBehaviour, ISerializationCallbackReceiver
        where T : class
    {
        private readonly HashSet<T> _listenersSet = new HashSet<T>();
        private readonly List<T> _listenersList = new List<T>();

#if UNITY_EDITOR
        [SerializeField]
        private List<ManagedField<T>> _listeners = new List<ManagedField<T>>();
#endif

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
        /// Remove all listeners from this service.
        /// </summary>
        public void RemoveAllListeners()
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

        void ISerializationCallbackReceiver.OnAfterDeserialize() { }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
#if UNITY_EDITOR
            _listeners.Clear();

            foreach (T listener in _listenersList)
            {
                _listeners.Add(listener);
            }
#endif
        }
    }
}
