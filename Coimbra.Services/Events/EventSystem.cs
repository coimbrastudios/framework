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
        /// <inheritdoc/>
        public event IEventService.EventHandler OnFirstListenerAdded;

        /// <inheritdoc/>
        public event IEventService.EventHandler OnLastListenerRemoved;

        private const string InvalidEventKeyMessageFormat = "The event key \"{0}\" doesn't match the current set key \"{1}\" for type \"{2}\"";

        private readonly Action<Type> _firstListenerAddedHandler;

        private readonly Action<Type> _lastListenerRemovedHandler;

        private readonly Dictionary<Type, Event> _events = new Dictionary<Type, Event>();

        private EventSystem()
        {
            _firstListenerAddedHandler = delegate(Type type)
            {
                OnFirstListenerAdded?.Invoke(this, type);
            };

            _lastListenerRemovedHandler = delegate(Type type)
            {
                OnLastListenerRemoved?.Invoke(this, type);
            };
        }

        /// <inheritdoc/>
        public ServiceLocator OwningLocator { get; set; }

        /// <summary>
        /// Create a new <see cref="IEventService"/>.
        /// </summary>
        public static IEventService Create()
        {
            return new EventSystem();
        }

        /// <inheritdoc/>
        public EventHandle AddListener<T>(Event<T>.Handler eventCallback)
            where T : IEvent
        {
            return AddListener(ref eventCallback);
        }

        /// <inheritdoc/>
        public bool AddListener<T>(Event<T>.Handler eventCallback, List<EventHandle> appendList)
            where T : IEvent
        {
            EventHandle eventHandle = AddListener(ref eventCallback);

            if (!eventHandle.IsValid)
            {
                return false;
            }

            appendList?.Add(eventHandle);

            return true;
        }

        /// <inheritdoc/>
        public bool CompareEventKey<T>(EventKey eventKey)
            where T : IEvent
        {
            return CompareEventKey(typeof(T), eventKey);
        }

        /// <inheritdoc/>
        public bool CompareEventKey(Type eventType, EventKey eventKey)
        {
            return _events.TryGetValue(eventType, out Event e) ? e.Key == eventKey : eventKey == null;
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
        public bool HasAnyListeners<T>()
            where T : IEvent
        {
            return HasAnyListeners(typeof(T));
        }

        /// <inheritdoc/>
        public bool HasAnyListeners(Type eventType)
        {
            return _events.TryGetValue(eventType, out Event e) && e.Count > 0;
        }

        /// <inheritdoc/>
        public bool HasListener(in EventHandle eventHandle)
        {
            return eventHandle.IsValid && _events.TryGetValue(eventHandle.Type, out Event e) && e.Contains(in eventHandle);
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
        public bool Invoke<T>(object eventSender, EventKey eventKey = null)
            where T : IEvent
        {
            Event<T> e = new Event<T>(this, eventSender);

            return Invoke(ref e, eventKey);
        }

        /// <inheritdoc/>
        public bool Invoke<T>(object eventSender, T eventData, EventKey eventKey = null)
            where T : IEvent
        {
            Event<T> e = new Event<T>(this, eventSender, ref eventData);

            return Invoke(ref e, eventKey);
        }

        /// <inheritdoc/>
        public bool Invoke<T>(object eventSender, ref T eventData, EventKey eventKey = null)
            where T : IEvent
        {
            Event<T> e = new Event<T>(this, eventSender, ref eventData);

            return Invoke(ref e, eventKey);
        }

        /// <inheritdoc/>
        public bool RemoveAllListeners<T>(EventKey eventKey = null)
            where T : IEvent
        {
            return RemoveAllListeners(typeof(T), eventKey);
        }

        /// <inheritdoc/>
        public bool RemoveAllListeners(Type eventType, EventKey eventKey = null)
        {
            if (!_events.TryGetValue(eventType, out Event e))
            {
                return false;
            }

            if (e.Key == null || e.Key == eventKey || (e.Key.Restrictions & EventKey.RestrictionOptions.DisallowRemoveAll) == 0)
            {
                return e.RemoveAllListeners();
            }

            Debug.LogErrorFormat(InvalidEventKeyMessageFormat, eventKey, e.Key, eventType);

            return false;
        }

        /// <inheritdoc/>
        public bool RemoveAllListenersAllowed()
        {
            bool result = false;

            foreach (Event e in _events.Values)
            {
                if (e.Key == null || (e.Key.Restrictions & EventKey.RestrictionOptions.DisallowRemoveAll) == 0)
                {
                    result |= e.RemoveAllListeners();
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public bool RemoveAllListenersWithKey(EventKey eventKey)
        {
            bool result = false;

            foreach (Event e in _events.Values)
            {
                if (e.Key == eventKey)
                {
                    result |= e.RemoveAllListeners();
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public bool RemoveListener(in EventHandle eventHandle)
        {
            return eventHandle.IsValid && _events.TryGetValue(eventHandle.Type, out Event e) && e.Remove(in eventHandle);
        }

        /// <inheritdoc/>
        public bool ResetAllEventKeys(EventKey eventKey)
        {
            bool result = false;

            foreach (Event e in _events.Values)
            {
                if (e.Key != eventKey)
                {
                    continue;
                }

                e.Key = null;
                result = true;
            }

            return result;
        }

        /// <inheritdoc/>
        public bool ResetEventKey<T>(EventKey eventKey)
            where T : IEvent
        {
            return ResetEventKey(typeof(T), eventKey);
        }

        /// <inheritdoc/>
        public bool ResetEventKey(Type eventType, EventKey eventKey)
        {
            if (eventKey == null || !_events.TryGetValue(eventType, out Event e))
            {
                return false;
            }

            if (e.Key != null && e.Key != eventKey)
            {
                Debug.LogErrorFormat(InvalidEventKeyMessageFormat, eventKey, e.Key, eventType);

                return false;
            }

            e.Key = null;

            return true;
        }

        /// <inheritdoc/>
        public bool SetEventKey<T>(EventKey eventKey)
            where T : IEvent
        {
            if (_events.TryGetValue(typeof(T), out Event e))
            {
                if (e.Key != null && e.Key != eventKey)
                {
                    Debug.LogErrorFormat(InvalidEventKeyMessageFormat, eventKey, e.Key, typeof(T));

                    return false;
                }
            }
            else
            {
                e = Create<T>();
            }

            e.Key = eventKey;

            return true;
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
            Event e = Event.Create<T>();
            e.OnFirstListenerAdded += _firstListenerAddedHandler;
            e.OnLastListenerRemoved += _lastListenerRemovedHandler;
            _events.Add(typeof(T), e);

            return e;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private EventHandle AddListener<T>(ref Event<T>.Handler eventCallback)
            where T : IEvent
        {
            if (eventCallback == null)
            {
                return default;
            }

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
        private bool Invoke<T>(ref Event<T> eventRef, EventKey eventKey = null)
            where T : IEvent
        {
            if (!_events.TryGetValue(typeof(T), out Event e))
            {
                return false;
            }

            if (e.Key != null && e.Key != eventKey && (e.Key.Restrictions & EventKey.RestrictionOptions.DisallowInvoke) != 0)
            {
                Debug.LogErrorFormat(InvalidEventKeyMessageFormat, eventKey, e.Key, typeof(T));

                return false;
            }

            if (e.IsInvoking)
            {
                Debug.LogError($"{typeof(T)} is already being invoked! Skipping its invocation to avoid a stack overflow.");

                return false;
            }

            int count = e.Count;

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
