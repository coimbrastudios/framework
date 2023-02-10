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
    /// <remarks>
    /// This implementation focus into <see cref="Invoke{T}"/> performance, with some key safety mechanisms:
    /// <list type="bullet">
    /// <item>All invocations are wrapped by a try/catch block to ensure that any thrown exceptions are logged with enough information to solve the issue.</item>
    /// <item>Invocations of a given event type within the same event type listeners is no-op (a warning message is logged by default, but can be disabled).</item>
    /// <item>Listeners are validated before being invoked to ensure that destroyed/null objects doesn't cause runtime errors (you can disable that validation to gain some performance at your own risk).</item>
    /// </list>
    /// Check the <see cref="EventSettings"/> for the customization options and the <b>Window/Coimbra Framework/Service Locator</b> window for useful debug information.
    /// Custom instances offers the same debug information as through the <see cref="ServiceLocator"/> as it is using a custom property drawer for that.
    /// </remarks>
    /// <seealso cref="IEvent"/>
    /// <seealso cref="IEventService"/>
    /// <seealso cref="EventHandle"/>
    /// <seealso cref="EventHandleTrackerComponent"/>
    /// <seealso cref="EventContext"/>
    /// <seealso cref="EventContextHandler{T}"/>
    /// <seealso cref="EventRelevancyChangedHandler"/>
    /// <seealso cref="EventSettings"/>
    [Serializable]
    public sealed class EventSystem : IEventService, ISerializationCallbackReceiver
    {
        private readonly Dictionary<Type, Event> _events = new();

#pragma warning disable CS0169
        [SerializeField]
        private List<Event>? _list;
#pragma warning restore CS0169

        internal IReadOnlyDictionary<Type, Event> Events => _events;

        /// <inheritdoc/>
        public EventHandle AddListener<T>(in EventContextHandler<T> eventHandler)
            where T : IEvent
        {
            EventHandle handle = new(this, typeof(T));
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
        private EventSettings GetOrCreateEventSettings()
        {
            if (ScriptableSettings.TryGetOrFind(out EventSettings eventSettings, ScriptableSettings.FindSingle))
            {
                return eventSettings;
            }

            eventSettings = ScriptableObject.CreateInstance<EventSettings>();
            ScriptableSettings.Set(eventSettings);

            return eventSettings;
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
                EventSettings eventSettings = GetOrCreateEventSettings();

                if (eventSettings.LogRecursiveInvocationWarning)
                {
                    Debug.LogWarning($"{typeof(T)} is already being invoked! Skipping its invocation to avoid a stack overflow.");
                }

                return false;
            }

            int listenerCount = e.ListenerCount;

            if (listenerCount == 0)
            {
                return false;
            }

            using (new Event.InvokeScope(e))
            {
                try
                {
                    EventSettings eventSettings = GetOrCreateEventSettings();

                    if (eventSettings.ValidateInvocationTargets)
                    {
                        InvokeSafely(ref eventContext, in eventData, e, listenerCount);
                    }
                    else
                    {
                        Invoke(ref eventContext, in eventData, e, listenerCount);
                    }
                }
                catch (Exception exception)
                {
                    Delegate handler = EventCallbacks<T>.Value[eventContext.CurrentHandle];
                    Debug.LogError($"An exception occurred while invoking {typeof(T)} for {handler.Target}.{handler.Method.Name}!", eventContext.Sender as UnityEngine.Object);
                    Debug.LogException(exception);
                }
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Invoke<T>(ref EventContext eventContext, in T eventData, Event e, int listenerCount)
            where T : IEvent
        {
            for (int i = 0; i < listenerCount; i++)
            {
                eventContext.CurrentHandle = e[i];

                if (!e.IsRemoving(eventContext.CurrentHandle))
                {
                    EventCallbacks<T>.Value[eventContext.CurrentHandle].Invoke(ref eventContext, in eventData);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InvokeSafely<T>(ref EventContext eventContext, in T eventData, Event e, int listenerCount)
            where T : IEvent
        {
            for (int i = 0; i < listenerCount; i++)
            {
                eventContext.CurrentHandle = e[i];

                if (e.IsRemoving(eventContext.CurrentHandle))
                {
                    continue;
                }

                EventContextHandler<T> listener = EventCallbacks<T>.Value[eventContext.CurrentHandle];

                if (listener.Method.IsStatic || listener.Target.IsValid())
                {
                    listener.Invoke(ref eventContext, in eventData);
                }
                else
                {
                    e.RemoveListener(eventContext.CurrentHandle);
                }
            }
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
            _list.Add(Event.Create<NullEvent>(null!));
#endif
        }
    }
}
