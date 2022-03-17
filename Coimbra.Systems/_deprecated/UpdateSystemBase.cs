using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coimbra.Systems
{
    /// <summary>
    /// Base implementation of the default implementations of update-based services.
    /// </summary>
    [AddComponentMenu("")]
    [Obsolete(nameof(UpdateSystemBase<TService, TListener>) + " has been deprecated. Any event implementation should go through the " + nameof(IEventService) + " instead.")]
    public abstract class UpdateSystemBase<TService, TListener> : MonoBehaviourServiceBase<TService>, ISerializationCallbackReceiver
        where TService : class, IService
        where TListener : class
    {
        private readonly HashSet<TListener> _listenersSet = new HashSet<TListener>();
        private readonly List<TListener> _listenersList = new List<TListener>();

#if UNITY_EDITOR
        [SerializeField]
        private List<ManagedField<TListener>> _listeners = new List<ManagedField<TListener>>();
#endif

        protected IReadOnlyList<TListener> Listeners => _listenersList;

        /// <inheritdoc/>
        protected override void OnDispose()
        {
            base.OnDispose();
            _listenersSet.Clear();
            _listenersList.Clear();
        }

        /// <summary>
        /// Add a listener to this service.
        /// </summary>
        public void AddListener(TListener listener)
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
        public void RemoveListener(TListener listener)
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

            foreach (TListener listener in _listenersList)
            {
                _listeners.Add(listener);
            }
#endif
        }
    }
}
