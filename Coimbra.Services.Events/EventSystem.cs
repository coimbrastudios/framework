#nullable enable

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Coimbra.Services.Events
{
    /// <summary>
    /// Default implementation for <see cref="IEventService"/>.
    /// </summary>
    [Serializable]
    public sealed class EventSystem : IEventService
    {
        private readonly Dictionary<Type, Event> _events = new Dictionary<Type, Event>();

        /// <inheritdoc/>
        [field: SerializeReference]
        [field: Disable]
        public ServiceLocator? OwningLocator { get; set; }

        /// <inheritdoc/>
        public EventHandle AddListener<T>(in Event<T>.Handler eventHandler)
            where T : IEvent
        {
            EventHandle handle = EventHandle.Create(typeof(T));
            EventCallbacks<T>.Value.Add(handle, eventHandler);

            if (!_events.TryGetValue(typeof(T), out Event e))
            {
                e = Create<T>();
            }

            e.Add(in handle);

            return handle;
        }

        /// <inheritdoc/>
        public void AddRelevancyListener<T>(in IEventService.EventRelevancyChangedHandler relevancyChangedHandler)
            where T : IEvent
        {
            if (!_events.TryGetValue(typeof(T), out Event e))
            {
                e = Create<T>();
            }

            e.OnRelevancyChanged += relevancyChangedHandler;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            foreach (Event e in _events.Values)
            {
                e.RemoveAllListeners();
            }

            _events.Clear();
        }

        /// <inheritdoc/>
        public int GetListenerCount<T>()
            where T : IEvent
        {
            return GetListenerCount(typeof(T));
        }

        /// <inheritdoc/>
        public int GetListenerCount(Type eventType)
        {
            return _events.TryGetValue(eventType, out Event e) ? e.ListenerCount : 0;
        }

        /// <inheritdoc/>
        public bool HasListener(in EventHandle eventHandle)
        {
            return eventHandle.IsValid && _events.TryGetValue(eventHandle.Type, out Event e) && e.HasListener(in eventHandle);
        }

        /// <inheritdoc/>
        public bool IsInvoking<T>()
            where T : IEvent
        {
            return _events.TryGetValue(typeof(T), out Event e) && e.IsInvoking;
        }

        /// <inheritdoc/>
        public bool IsInvoking(Type eventType)
        {
            return _events.TryGetValue(eventType, out Event e) && e.IsInvoking;
        }

        /// <inheritdoc/>
        public bool Invoke<T>(object eventSender)
            where T : IEvent
        {
            Event<T> e = new Event<T>(this, eventSender);

            return Invoke(ref e);
        }

        /// <inheritdoc/>
        public bool Invoke<T>(object eventSender, in T eventData)
            where T : IEvent
        {
            Event<T> e = new Event<T>(this, eventSender, in eventData);

            return Invoke(ref e);
        }

        /// <inheritdoc/>
        public bool RemoveAllListeners<T>()
            where T : IEvent
        {
            return _events.TryGetValue(typeof(T), out Event e) && e.RemoveAllListeners();
        }

        /// <inheritdoc/>
        public bool RemoveListener(in EventHandle eventHandle)
        {
            return eventHandle.IsValid && _events.TryGetValue(eventHandle.Type, out Event e) && e.RemoveListener(in eventHandle);
        }

        /// <inheritdoc/>
        public void RemoveRelevancyListener<T>(in IEventService.EventRelevancyChangedHandler relevancyChangedHandler)
            where T : IEvent
        {
            if (_events.TryGetValue(typeof(T), out Event e))
            {
                e.OnRelevancyChanged -= relevancyChangedHandler;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Event Create<T>()
            where T : IEvent
        {
            Event e = Event.Create<T>(this);
            _events.Add(typeof(T), e);

            return e;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Invoke<T>(ref Event<T> eventRef)
            where T : IEvent
        {
            if (!_events.TryGetValue(typeof(T), out Event e))
            {
                return false;
            }

            if (e.IsInvoking)
            {
                Debug.LogError($"{typeof(T)} is already being invoked! Skipping its invocation to avoid a stack overflow.");

                return false;
            }

            int count = e.ListenerCount;

            if (count == 0)
            {
                return false;
            }

            using (new Event.InvokeScope(e))
            {
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        eventRef.CurrentHandle = e[i];

                        if (!e.IsRemoving(eventRef.CurrentHandle))
                        {
                            EventCallbacks<T>.Value[eventRef.CurrentHandle].Invoke(ref eventRef);
                        }
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogError($"An exception occurred while invoking {typeof(T)}!", eventRef.Sender as Object);
                    Debug.LogException(exception);
                }
            }

            return true;
        }
    }
}
