using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Coimbra
{
    internal sealed class EventSystem : IEventService
    {
        private sealed class Event
        {
            internal readonly RemoveHandler RemoveHandler;

            internal readonly List<EventHandle> Handles = new List<EventHandle>();

            internal readonly List<EventHandle> HandlesToAdd = new List<EventHandle>();

            internal readonly HashSet<EventHandle> HandlesToRemove = new HashSet<EventHandle>();

            internal object Key;

            internal bool IsInvoking;

            internal Event(RemoveHandler removeHandler, object key = null)
            {
                RemoveHandler = removeHandler;
                Key = key;
            }
        }

        private static class EventCallbacks<T>
        {
            internal static readonly Dictionary<EventHandle, EventListenerHandler<T>> Value = new Dictionary<EventHandle, EventListenerHandler<T>>(1);
            internal static readonly RemoveHandler RemoveHandler = Value.Remove;
        }

        private delegate bool RemoveHandler(EventHandle key);

        private const string InvalidEventKeyMessageFormat = "The event key \"{0}\" doesn't match the current set key \"{1}\" for type \"{2}\"";
        private readonly object _serviceKey;
        private readonly Dictionary<Type, Event> _events = new Dictionary<Type, Event>();

        private EventSystem(object serviceKey = null)
        {
            _serviceKey = serviceKey;
        }

        /// <inheritdoc/>
        public ServiceLocator OwningLocator { get; set; }

        /// <inheritdoc/>
        public EventHandle AddListener<T>(EventListenerHandler<T> eventCallback)
            where T : IEvent
        {
            CheckType(typeof(T));

            if (eventCallback == null)
            {
                return new EventHandle();
            }

            EventHandle handle = EventHandle.Create(typeof(T));
            EventCallbacks<T>.Value.Add(handle, eventCallback);

            if (!_events.TryGetValue(typeof(T), out Event e))
            {
                e = new Event(EventCallbacks<T>.RemoveHandler);
                e.Handles.Add(handle);
                _events.Add(typeof(T), e);
            }
            else if (e.IsInvoking)
            {
                e.HandlesToAdd.Add(handle);
            }
            else
            {
                e.Handles.Add(handle);
            }

            return handle;
        }

        /// <inheritdoc/>
        public bool AddListener<T>(EventListenerHandler<T> eventCallback, List<EventHandle> appendList)
            where T : IEvent
        {
            EventHandle eventHandle = AddListener(eventCallback);

            if (!eventHandle.IsValid)
            {
                return false;
            }

            appendList?.Add(eventHandle);

            return true;
        }

        /// <inheritdoc/>
        public bool CompareEventKey<T>(object eventKey)
            where T : IEvent
        {
            return CompareEventKey(typeof(T), eventKey);
        }

        /// <inheritdoc/>
        public bool CompareEventKey(Type eventType, object eventKey)
        {
            CheckType(eventType);

            return _events.TryGetValue(eventType, out Event e) ? e.Key == eventKey : eventKey == null;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            RemoveAllListeners(_serviceKey);
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
            CheckType(eventType);

            return _events.TryGetValue(eventType, out Event e) && e.Handles.Count > 0;
        }

        /// <inheritdoc/>
        public bool HasListener(in EventHandle eventHandle)
        {
            return eventHandle.IsValid && _events.TryGetValue(eventHandle.Type, out Event e) && e.Handles.Contains(eventHandle);
        }

        /// <inheritdoc/>
        public bool Invoke<T>(object eventSender, object eventKey = null)
            where T : IEvent, new()
        {
            T eventData = new T();

            return Invoke(eventSender, ref eventData, eventKey);
        }

        /// <inheritdoc/>
        public bool Invoke<T>(object eventSender, T eventData, object eventKey = null)
            where T : IEvent
        {
            return Invoke(eventSender, ref eventData, eventKey);
        }

        /// <inheritdoc/>
        public bool Invoke<T>(object eventSender, ref T eventData, object eventKey = null)
            where T : IEvent
        {
            CheckType(typeof(T));

            if (!_events.TryGetValue(typeof(T), out Event e))
            {
                return false;
            }

            if (e.Key != null && e.Key != eventKey)
            {
                Debug.LogErrorFormat(InvalidEventKeyMessageFormat, eventKey, e.Key, typeof(T));

                return false;
            }

            if (e.IsInvoking)
            {
                Debug.LogError($"{typeof(T)} is already being invoked! Skipping its invocation to avoid a stack overflow.");

                return false;
            }

            e.IsInvoking = true;

            foreach (EventHandle handle in e.Handles)
            {
                if (!e.HandlesToRemove.Contains(handle))
                {
                    EventCallbacks<T>.Value[handle].Invoke(eventSender, ref eventData);
                }
            }

            e.IsInvoking = false;

            foreach (EventHandle handle in e.HandlesToRemove)
            {
                e.Handles.Remove(handle);
                e.RemoveHandler.Invoke(handle);
            }

            e.HandlesToRemove.Clear();

            foreach (EventHandle handle in e.HandlesToAdd)
            {
                e.Handles.Add(handle);
            }

            e.HandlesToAdd.Clear();

            return true;
        }

        /// <inheritdoc/>
        public bool RemoveAllListeners(object serviceKey = null)
        {
            if (_serviceKey != null && _serviceKey != serviceKey)
            {
                Debug.LogError($"The service key \"{serviceKey}\" doesn't match the current set key \"{_serviceKey}\"");

                return false;
            }

            bool result = false;

            foreach (Event e in _events.Values)
            {
                if (e.IsInvoking)
                {
                    foreach (EventHandle handle in e.Handles)
                    {
                        result |= e.HandlesToRemove.Add(handle);
                    }
                }
                else
                {
                    foreach (EventHandle handle in e.Handles)
                    {
                        result |= e.RemoveHandler.Invoke(handle);
                    }

                    e.Handles.Clear();
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public bool RemoveAllListeners<T>(object eventKey = null)
            where T : IEvent
        {
            return RemoveAllListeners(typeof(T), eventKey);
        }

        /// <inheritdoc/>
        public bool RemoveAllListeners(Type eventType, object eventKey = null)
        {
            CheckType(eventType);

            if (!_events.TryGetValue(eventType, out Event e))
            {
                return false;
            }

            if (e.Key != null && e.Key != eventKey)
            {
                Debug.LogErrorFormat(InvalidEventKeyMessageFormat, eventKey, e.Key, eventType);

                return false;
            }

            bool result = false;

            if (e.IsInvoking)
            {
                foreach (EventHandle handle in e.Handles)
                {
                    result |= e.HandlesToRemove.Add(handle);
                }
            }
            else
            {
                foreach (EventHandle handle in e.Handles)
                {
                    result |= e.RemoveHandler.Invoke(handle);
                }

                e.Handles.Clear();
            }

            return result;
        }

        /// <inheritdoc/>
        public bool RemoveListener(in EventHandle eventHandle)
        {
            if (!eventHandle.IsValid || !_events.TryGetValue(eventHandle.Type, out Event e))
            {
                return false;
            }

            if (e.IsInvoking)
            {
                return e.HandlesToRemove.Add(eventHandle);
            }

            return e.RemoveHandler.Invoke(eventHandle) && e.Handles.Remove(eventHandle);
        }

        /// <inheritdoc/>
        public bool ResetEventKey<T>(object eventKey)
            where T : IEvent
        {
            return ResetEventKey(typeof(T), eventKey);
        }

        /// <inheritdoc/>
        public bool ResetEventKey(Type eventType, object eventKey)
        {
            CheckType(eventType);

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
        public bool SetEventKey<T>(object eventKey)
            where T : IEvent
        {
            CheckType(typeof(T));

            if (_events.TryGetValue(typeof(T), out Event e))
            {
                if (e.Key != null && e.Key != eventKey)
                {
                    Debug.LogErrorFormat(InvalidEventKeyMessageFormat, eventKey, e.Key, typeof(T));

                    return false;
                }

                e.Key = eventKey;
            }
            else
            {
                _events.Add(typeof(T), new Event(EventCallbacks<T>.RemoveHandler, eventKey));
            }

            return true;
        }

        internal static IEventService Create()
        {
            return new EventSystem();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            ServiceLocator.Shared.SetCreateCallback(Create, false);
        }

        [Conditional("UNITY_EDITOR")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CheckType(Type type, [CallerMemberName] string memberName = null)
        {
            if (type.IsInterface)
            {
                throw new ArgumentOutOfRangeException($"\"{nameof(IEventService)}.{memberName}\" requires a non-interface type argument!");
            }

            if (!typeof(IEvent).IsAssignableFrom(type))
            {
                throw new ArgumentOutOfRangeException($"\"{nameof(IEventService)}.{memberName}\" requires a type that implements \"{typeof(IEvent)}\"!");
            }
        }
    }
}
