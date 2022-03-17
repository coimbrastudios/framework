using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coimbra.Systems
{
    /// <summary>
    /// Default implementation for <see cref="IEventService"/>.
    /// </summary>
    public class EventSystem : IEventService
    {
        private sealed class Event
        {
            internal readonly RemoveHandler RemoveHandler;

            internal readonly List<EventHandle> Handles = new List<EventHandle>();

            internal object Key;

            internal Event(RemoveHandler removeHandler, object key = null)
            {
                RemoveHandler = removeHandler;
                Key = key;
            }
        }

        private static class EventCallbacks<T>
        {
            internal static readonly Dictionary<EventHandle, EventRefHandler<T>> Value = new Dictionary<EventHandle, EventRefHandler<T>>(1);
            internal static readonly RemoveHandler RemoveHandler = Value.Remove;
        }

        private delegate bool RemoveHandler(EventHandle key);

        private readonly object _serviceKey;
        private readonly Dictionary<Type, Event> _events = new Dictionary<Type, Event>();

        public EventSystem(object serviceKey = null)
        {
            _serviceKey = serviceKey;
        }

        /// <inheritdoc cref="IService.OwningLocator"/>
        public ServiceLocator OwningLocator { get; set; }

        /// <inheritdoc cref="IEventService.AddListener{T}(EventHandler{T})"/>.
        public EventHandle AddListener<T>(EventHandler<T> eventCallback)
        {
            if (eventCallback == null)
            {
                return new EventHandle();
            }

            return AddListener(delegate(object sender, ref T e)
            {
                eventCallback.Invoke(sender, e);
            });
        }

        /// <inheritdoc cref="IEventService.AddListener{T}(EventRefHandler{T})"/>.
        public EventHandle AddListener<T>(EventRefHandler<T> eventCallback)
        {
            if (eventCallback == null)
            {
                return new EventHandle();
            }

            EventHandle handle = EventHandle.Create(typeof(T));
            EventCallbacks<T>.Value.Add(handle, eventCallback);

            if (!_events.TryGetValue(typeof(T), out Event e))
            {
                e = new Event(EventCallbacks<T>.RemoveHandler);
                _events.Add(typeof(T), e);
            }

            e.Handles.Add(handle);

            return handle;
        }

        /// <inheritdoc cref="IEventService.CompareEventKey{T}"/>.
        public bool CompareEventKey<T>(object eventKey)
        {
            return CompareEventKey(typeof(T), eventKey);
        }

        /// <inheritdoc cref="IEventService.CompareEventKey"/>.
        public bool CompareEventKey(Type eventType, object eventKey)
        {
            return _events.TryGetValue(eventType, out Event e) ? e.Key == eventKey : eventKey == null;
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            RemoveAllListeners(_serviceKey);
            _events.Clear();
        }

        /// <inheritdoc cref="IEventService.HasAnyListeners{T}"/>.
        public bool HasAnyListeners<T>()
        {
            return HasAnyListeners(typeof(T));
        }

        /// <inheritdoc cref="IEventService.HasAnyListeners"/>.
        public bool HasAnyListeners(Type eventType)
        {
            return _events.TryGetValue(eventType, out Event e) && e.Handles.Count > 0;
        }

        /// <inheritdoc cref="IEventService.HasListener"/>.
        public bool HasListener(in EventHandle eventHandle)
        {
            return eventHandle.IsValid && _events.TryGetValue(eventHandle.Type, out Event e) && e.Handles.Contains(eventHandle);
        }

        /// <inheritdoc cref="IEventService.Invoke"/>.
        public void Invoke(object eventSender, Type eventType, object eventKey = null, bool ignoreException = false)
        {
            Invoke(eventSender, Activator.CreateInstance(eventType), eventKey, ignoreException);
        }

        /// <inheritdoc cref="IEventService.Invoke{T}(object, T, object, bool)"/>.
        public void Invoke<T>(object eventSender, T eventData, object eventKey = null, bool ignoreException = false)
        {
            Invoke(eventSender, ref eventData, eventKey, ignoreException);
        }

        /// <inheritdoc cref="IEventService.Invoke{T}(object, ref T, object, bool)"/>.
        public void Invoke<T>(object eventSender, ref T eventData, object eventKey = null, bool ignoreException = false)
        {
            if (!_events.TryGetValue(typeof(T), out Event e))
            {
                return;
            }

            if (e.Key != null && e.Key != eventKey)
            {
                if (ignoreException)
                {
                    return;
                }

                throw new InvalidOperationException();
            }

            foreach (EventHandle handle in e.Handles)
            {
                EventCallbacks<T>.Value[handle].Invoke(eventSender, ref eventData);
            }
        }

        /// <inheritdoc cref="IEventService.RemoveAllListeners(object, bool)"/>.
        public void RemoveAllListeners(object serviceKey = null, bool ignoreException = false)
        {
            if (_serviceKey != null && _serviceKey != serviceKey)
            {
                if (ignoreException)
                {
                    return;
                }

                throw new InvalidOperationException();
            }

            foreach (Event e in _events.Values)
            {
                foreach (EventHandle handle in e.Handles)
                {
                    e.RemoveHandler.Invoke(handle);
                }

                e.Handles.Clear();
            }
        }

        /// <inheritdoc cref="IEventService.RemoveAllListeners{T}"/>.
        public void RemoveAllListeners<T>(object eventKey = null, bool ignoreException = false)
        {
            RemoveAllListeners(typeof(T), eventKey, ignoreException);
        }

        /// <inheritdoc cref="IEventService.RemoveAllListeners(Type, object, bool)"/>.
        public void RemoveAllListeners(Type eventType, object eventKey = null, bool ignoreException = false)
        {
            if (!_events.TryGetValue(eventType, out Event e))
            {
                return;
            }

            if (e.Key != null && e.Key != eventKey)
            {
                if (ignoreException)
                {
                    return;
                }

                throw new InvalidOperationException();
            }

            foreach (EventHandle handle in e.Handles)
            {
                e.RemoveHandler.Invoke(handle);
            }

            e.Handles.Clear();
        }

        /// <inheritdoc cref="IEventService.RemoveListener"/>.
        public void RemoveListener(in EventHandle eventHandle)
        {
            if (!_events.TryGetValue(eventHandle.Type, out Event e))
            {
                return;
            }

            if (e.RemoveHandler.Invoke(eventHandle))
            {
                e.Handles.Remove(eventHandle);
            }
        }

        /// <inheritdoc cref="IEventService.ResetEventKey{T}"/>.
        public void ResetEventKey<T>(object eventKey)
        {
            ResetEventKey(typeof(T), eventKey);
        }

        /// <inheritdoc cref="IEventService.ResetEventKey"/>.
        public void ResetEventKey(Type eventType, object eventKey)
        {
            if (eventKey == null || !_events.TryGetValue(eventType, out Event e))
            {
                return;
            }

            if (e.Key != null && e.Key != eventKey)
            {
                throw new InvalidOperationException();
            }

            e.Key = null;
        }

        /// <inheritdoc cref="IEventService.SetEventKey{T}"/>.
        public void SetEventKey<T>(object eventKey)
        {
            if (_events.TryGetValue(typeof(T), out Event e))
            {
                if (e.Key != null && e.Key != eventKey)
                {
                    throw new InvalidOperationException();
                }

                e.Key = eventKey;
            }
            else
            {
                _events.Add(typeof(T), new Event(EventCallbacks<T>.RemoveHandler, eventKey));
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            ServiceLocator.SetDefaultCreateCallback(Create, false);
        }

        private static IEventService Create()
        {
            return new EventSystem();
        }
    }
}
