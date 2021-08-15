using System;
using System.Collections;
using System.Collections.Generic;

namespace Coimbra
{
    /// <summary>
    /// Event service that also has encapsulation for some of its methods.
    /// </summary>
    public interface IEventService : IDisposable
    {
        /// <summary>
        /// Adds a listener to an event type.
        /// </summary>
        /// <param name="callback">The callback to be added.</param>
        /// <typeparam name="T">The event type.</typeparam>
        void AddListener<T>(EventHandler<T> callback);

        /// <summary>
        /// Invokes the specified event type for all its listeners.
        /// </summary>
        /// <param name="sender">The object invoking the event.</param>
        /// <param name="eventData">The event data to be sent.</param>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        /// <typeparam name="T">The event type.</typeparam>
        void Invoke<T>(object sender, T eventData, object eventKey = null);

        /// <summary>
        /// Removes a listener from an event type.
        /// </summary>
        /// <param name="callback">The callback to be removed.</param>
        /// <typeparam name="T">The event type.</typeparam>
        void RemoveListener<T>(EventHandler<T> callback);

        /// <summary>
        /// Removes all listeners from all event types.
        /// </summary>
        /// <param name="serviceKey">The encapsulation key for the service.</param>
        void RemoveAllListeners(object serviceKey = null);

        /// <summary>
        /// Removes all listeners from an event type.
        /// </summary>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        /// <typeparam name="T">The event type.</typeparam>
        void RemoveAllListeners<T>(object eventKey = null);

        /// <summary>
        /// Resets the encapsulation key for an event type;
        /// </summary>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        /// <typeparam name="T">The event type.</typeparam>
        void ResetEventKey<T>(object eventKey);

        /// <summary>
        /// Sets the encapsulation key for an event type;
        /// </summary>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        /// <typeparam name="T">The event type.</typeparam>
        void SetEventKey<T>(object eventKey);
    }

    /// <summary>
    /// Default implementation for <see cref="IEventService"/>.
    /// </summary>
    public sealed class EventService : IEventService
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
    }
}
