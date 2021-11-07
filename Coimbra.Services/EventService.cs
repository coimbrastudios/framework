using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coimbra.Services
{
    /// <summary>
    /// Default implementation for <see cref="IEventService"/>.
    /// </summary>
    public class EventService : IEventService
    {
        private static class EventServiceT<T>
        {
            internal static readonly Dictionary<EventService, EventHandler<T>> Callbacks = new Dictionary<EventService, EventHandler<T>>(1);
        }

        private readonly object _serviceKey;
        private readonly Dictionary<Type, object> _eventKeys = new Dictionary<Type, object>();
        private readonly HashSet<IDictionary> _dependencies = new HashSet<IDictionary>();

        public EventService(object serviceKey = null)
        {
            _serviceKey = serviceKey;
        }

        /// <inheritdoc cref="IEventService.AddListener{T}"/>.
        public void AddListener<T>(EventHandler<T> callback)
        {
            if (_dependencies.Add(EventServiceT<T>.Callbacks))
            {
                EventServiceT<T>.Callbacks[this] = callback;
            }
            else
            {
                EventServiceT<T>.Callbacks[this] += callback;
            }
        }

        /// <inheritdoc cref="IEventService.Invoke{T}"/>.
        public void Invoke<T>(object sender, T eventData, object eventKey = null)
        {
            if (_eventKeys.TryGetValue(typeof(T), out object value) && eventKey != value)
            {
                throw new InvalidOperationException();
            }

            if (EventServiceT<T>.Callbacks.TryGetValue(this, out EventHandler<T> eventHandler))
            {
                eventHandler?.Invoke(sender, eventData);
            }
        }

        /// <inheritdoc cref="IEventService.RemoveListener{T}"/>.
        public void RemoveListener<T>(EventHandler<T> callback)
        {
            if (EventServiceT<T>.Callbacks.TryGetValue(this, out EventHandler<T> eventHandler))
            {
                EventServiceT<T>.Callbacks[this] = eventHandler - callback;
            }
        }

        /// <inheritdoc cref="IEventService.RemoveAllListeners"/>.
        public void RemoveAllListeners(object serviceKey = null)
        {
            if (_serviceKey != null && _serviceKey != serviceKey)
            {
                throw new InvalidOperationException();
            }

            foreach (IDictionary dependency in _dependencies)
            {
                dependency.Remove(this);
            }

            _dependencies.Clear();
        }

        /// <inheritdoc cref="IEventService.RemoveAllListeners{T}"/>.
        public void RemoveAllListeners<T>(object eventKey = null)
        {
            if (_eventKeys.TryGetValue(typeof(T), out object value) && eventKey != value)
            {
                throw new InvalidOperationException();
            }

            if (_dependencies.Remove(EventServiceT<T>.Callbacks))
            {
                EventServiceT<T>.Callbacks.Remove(this);
            }
        }

        /// <inheritdoc cref="IEventService.ResetEventKey{T}"/>.
        public void ResetEventKey<T>(object eventKey)
        {
            if (_eventKeys.TryGetValue(typeof(T), out object value) && eventKey != value)
            {
                throw new InvalidOperationException();
            }

            _eventKeys.Remove(typeof(T));
        }

        /// <inheritdoc cref="IEventService.SetEventKey{T}"/>.
        public void SetEventKey<T>(object eventKey)
        {
            if (_eventKeys.TryGetValue(typeof(T), out object value) && eventKey != value)
            {
                throw new InvalidOperationException();
            }

            _eventKeys[typeof(T)] = eventKey;
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            RemoveAllListeners(_serviceKey);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            ServiceLocator.SetDefaultCreateCallback(Create, false);
        }

        private static IEventService Create()
        {
            return new EventService();
        }
    }
}
