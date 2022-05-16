#nullable enable

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Coimbra.Services.Events
{
    /// <summary>
    /// Default implementation for <see cref="IEventService"/>.
    /// </summary>
    public sealed class EventSystem : IEventService
    {
        private readonly Dictionary<Type, Event> _events = new Dictionary<Type, Event>();

        /// <inheritdoc/>
        public ServiceLocator? OwningLocator { get; set; }

        /// <summary>
        /// Create a new <see cref="IEventService"/>.
        /// </summary>
        public static IEventService Create()
        {
            return new EventSystem();
        }

        /// <inheritdoc/>
        public EventHandle AddListener<T>(Event<T>.Handler eventHandler)
            where T : IEvent
        {
            return AddListener(ref eventHandler);
        }

        /// <inheritdoc/>
        public bool AddListener<T>(Event<T>.Handler eventHandler, List<EventHandle> appendList)
            where T : IEvent
        {
            EventHandle eventHandle = AddListener(ref eventHandler);

            if (!eventHandle.IsValid)
            {
                return false;
            }

            appendList.Add(eventHandle);

            return true;
        }

        /// <inheritdoc/>
        public void AddRelevancyListener<T>(IEventService.EventRelevancyChangedHandler relevancyChangedHandler)
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
        public bool Invoke<T>(object eventSender, ref T eventData)
            where T : IEvent
        {
            Event<T> e = new Event<T>(this, eventSender, ref eventData);

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
        public void RemoveRelevancyListener<T>(IEventService.EventRelevancyChangedHandler relevancyChangedHandler)
            where T : IEvent
        {
            if (_events.TryGetValue(typeof(T), out Event e))
            {
                e.OnRelevancyChanged -= relevancyChangedHandler;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void HandleSubsystemRegistration()
        {
            ServiceLocator.Shared.SetCreateCallback(Create, false);
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
        private EventHandle AddListener<T>(ref Event<T>.Handler eventCallback)
            where T : IEvent
        {
            EventHandle handle = EventHandle.Create(typeof(T));
            EventCallbacks<T>.Value.Add(handle, eventCallback);

            if (!_events.TryGetValue(typeof(T), out Event e))
            {
                e = Create<T>();
            }

            e.Add(in handle);

            return handle;
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
                    Debug.LogException(new Exception($"An exception occurred while invoking {typeof(T)}!", exception));
                }
            }

            return true;
        }
    }
}
