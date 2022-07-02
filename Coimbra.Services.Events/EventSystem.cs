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
    public sealed class EventSystem : IEventService, ISerializationCallbackReceiver
    {
        private readonly Dictionary<Type, Event> _events = new();

        [SerializeField]
        private List<Event>? _list;

        internal IReadOnlyDictionary<Type, Event> Events => _events;

        /// <inheritdoc/>
        public EventHandle AddListener<T>(in EventContextHandler<T> eventHandler)
            where T : IEvent
        {
            EventHandle handle = EventHandle.Create(this, typeof(T));
            EventCallbacks<T>.Value.Add(handle, eventHandler);

            if (!_events.TryGetValue(typeof(T), out Event e))
            {
                e = Create<T>();
            }

            e.Add(in handle);

            return handle;
        }

        /// <inheritdoc/>
        public void AddRelevancyListener<T>(in EventRelevancyChangedHandler relevancyChangedHandler)
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
        public int GetListeners(in EventHandle eventHandle, List<DelegateListener> listeners)
        {
            return eventHandle.Type != null && _events.TryGetValue(eventHandle.Type, out Event e) ? e.GetListenersHandler(eventHandle, listeners) : 0;
        }

        /// <inheritdoc/>
        public int GetListeners<TEvent>(List<DelegateListener> listeners)
            where TEvent : IEvent
        {
            return GetListeners(typeof(TEvent), listeners);
        }

        /// <inheritdoc/>
        public int GetListeners(Type eventType, List<DelegateListener> listeners)
        {
            return _events.TryGetValue(eventType, out Event e) ? e.GetListeners(listeners) : 0;
        }

        /// <inheritdoc/>
        public int GetRelevancyListeners<TEvent>(List<DelegateListener> listeners)
            where TEvent : IEvent
        {
            return GetRelevancyListeners(typeof(TEvent), listeners);
        }

        /// <inheritdoc/>
        public int GetRelevancyListeners(Type eventType, List<DelegateListener> listeners)
        {
            return _events.TryGetValue(eventType, out Event e) ? e.GetRelevancyListeners(listeners) : 0;
        }

        /// <inheritdoc/>
        public bool HasListener(in EventHandle eventHandle)
        {
            return eventHandle.Type != null && _events.TryGetValue(eventHandle.Type, out Event e) && e.HasListener(in eventHandle);
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
        public bool Invoke<T>(object eventSender, in T eventData)
            where T : IEvent
        {
            EventContext eventContext = new(this, eventSender, typeof(T));

            return Invoke(ref eventContext, in eventData);
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
            return eventHandle.Type != null && _events.TryGetValue(eventHandle.Type, out Event e) && e.RemoveListener(in eventHandle);
        }

        /// <inheritdoc/>
        public void RemoveRelevancyListener<T>(in EventRelevancyChangedHandler relevancyChangedHandler)
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
        private bool Invoke<T>(ref EventContext eventContext, in T eventData)
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
                        eventContext.CurrentHandle = e[i];

                        if (!e.IsRemoving(eventContext.CurrentHandle))
                        {
                            EventCallbacks<T>.Value[eventContext.CurrentHandle].Invoke(ref eventContext, in eventData);
                        }
                    }
                }
                catch (Exception exception)
                {
                    Delegate handler = EventCallbacks<T>.Value[eventContext.CurrentHandle];
                    Debug.LogError($"An exception occurred while invoking {typeof(T)} for {handler.Target}.{handler.Method.Name}!", eventContext.Sender as Object);
                    Debug.LogException(exception);
                }
            }

            return true;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() { }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (_list == null)
            {
                _list = new List<Event>();
            }
            else
            {
                _list.Clear();
            }

            _list.AddRange(_events.Values);
#endif
        }
    }
}
