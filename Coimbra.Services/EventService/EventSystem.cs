using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Coimbra.Services
{
    /// <summary>
    /// Default implementation for <see cref="IEventService"/>.
    /// </summary>
    public sealed class EventSystem : IEventService
    {
        private delegate bool RemoveHandler(EventHandle key);

        private static class EventCallbacks<T>
            where T : IEvent
        {
            internal static readonly Dictionary<EventHandle, EventData<T>.Handler> Value = new(1);

            internal static readonly RemoveHandler RemoveHandler = Value.Remove;
        }

        private sealed class Event
        {
            internal readonly RemoveHandler RemoveHandler;

            internal readonly List<EventHandle> Handles = new();

            internal readonly HashSet<EventHandle> HandlesToRemove = new();

            internal bool IsInvoking;

            internal EventKey Key;

            private Event(RemoveHandler removeHandler)
            {
                RemoveHandler = removeHandler;
            }

            public static Event Create<T>()
                where T : IEvent
            {
                return new Event(EventCallbacks<T>.RemoveHandler);
            }
        }

        private const string InvalidEventKeyMessageFormat = "The event key \"{0}\" doesn't match the current set key \"{1}\" for type \"{2}\"";

        private readonly Dictionary<Type, Event> _events = new();

        private EventSystem() { }

        /// <inheritdoc/>
        public ServiceLocator OwningLocator { get; set; }

        /// <summary>
        /// Create a new <see cref="IEventService"/>.
        /// </summary>
        public static IEventService Create()
        {
            return new EventSystem();
        }

        /// <inheritdoc cref="IEventService.AddListener{T}(Coimbra.Services.EventData{T}.Handler)"/>
        public EventHandle AddListener<T>(EventData<T>.Handler eventCallback)
            where T : IEvent
        {
            return AddListener(ref eventCallback);
        }

        /// <inheritdoc cref="IEventService.AddListener{T}(Coimbra.Services.EventData{T}.Handler, List{EventHandle})"/>
        public bool AddListener<T>(EventData<T>.Handler eventCallback, List<EventHandle> appendList)
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
            eventType.AssertNonInterfaceImplements<IEvent>();

            return _events.TryGetValue(eventType, out Event e) ? e.Key == eventKey : eventKey == null;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            foreach (Event e in _events.Values)
            {
                RemoveAllListeners(e);
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
            eventType.AssertNonInterfaceImplements<IEvent>();

            return _events.TryGetValue(eventType, out Event e) && e.Handles.Count > 0;
        }

        /// <inheritdoc/>
        public bool HasListener(in EventHandle eventHandle)
        {
            return eventHandle.IsValid && _events.TryGetValue(eventHandle.Type, out Event e) && e.Handles.Contains(eventHandle);
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
            where T : IEvent, new()
        {
            EventData<T> eventData = new(eventSender);

            return Invoke(ref eventData, eventKey);
        }

        /// <inheritdoc/>
        public bool Invoke<T>(object eventSender, T eventValue, EventKey eventKey = null)
            where T : IEvent
        {
            EventData<T> eventData = new(eventSender, ref eventValue);

            return Invoke(ref eventData, eventKey);
        }

        /// <inheritdoc/>
        public bool Invoke<T>(object eventSender, ref T eventValue, EventKey eventKey = null)
            where T : IEvent
        {
            EventData<T> eventData = new(eventSender, ref eventValue);

            return Invoke(ref eventData, eventKey);
        }

        /// <inheritdoc/>
        public bool Invoke<T>(EventData<T> eventData, EventKey eventKey = null)
            where T : IEvent
        {
            return Invoke(ref eventData, eventKey);
        }

        /// <inheritdoc/>
        public bool Invoke<T>(ref EventData<T> eventData, EventKey eventKey = null)
            where T : IEvent
        {
            typeof(T).AssertNonInterfaceImplements<IEvent>();

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

            int count = e.Handles.Count;

            if (count == 0)
            {
                return false;
            }

            e.IsInvoking = true;

            try
            {
                for (int i = 0; i < count; i++)
                {
                    eventData.CurrentHandle = e.Handles[i];

                    if (!e.HandlesToRemove.Contains(eventData.CurrentHandle))
                    {
                        EventCallbacks<T>.Value[eventData.CurrentHandle].Invoke(ref eventData);
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(new Exception($"An exception occurred while invoking {typeof(T)}!", exception));
            }

            e.IsInvoking = false;

            foreach (EventHandle handle in e.HandlesToRemove)
            {
                e.Handles.Remove(handle);
                e.RemoveHandler.Invoke(handle);
            }

            e.HandlesToRemove.Clear();

            return true;
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
            eventType.AssertNonInterfaceImplements<IEvent>();

            if (!_events.TryGetValue(eventType, out Event e))
            {
                return false;
            }

            if (e.Key != null && e.Key != eventKey && (e.Key.Restrictions & EventKey.RestrictionOptions.DisallowRemoveAll) != 0)
            {
                Debug.LogErrorFormat(InvalidEventKeyMessageFormat, eventKey, e.Key, eventType);

                return false;
            }

            return RemoveAllListeners(e);
        }

        /// <inheritdoc/>
        public bool RemoveAllListenersAllowed()
        {
            bool result = false;

            foreach (Event e in _events.Values)
            {
                if (e.Key == null || (e.Key.Restrictions & EventKey.RestrictionOptions.DisallowRemoveAll) == 0)
                {
                    result |= RemoveAllListeners(e);
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
                    result |= RemoveAllListeners(e);
                }
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
            eventType.AssertNonInterfaceImplements<IEvent>();

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
            typeof(T).AssertNonInterfaceImplements<IEvent>();

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
                e = Event.Create<T>();
                _events.Add(typeof(T), e);
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
        private static bool RemoveAllListeners(Event e)
        {
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private EventHandle AddListener<T>(ref EventData<T>.Handler eventCallback)
            where T : IEvent
        {
            typeof(T).AssertNonInterfaceImplements<IEvent>();

            if (eventCallback == null)
            {
                return default;
            }

            EventHandle handle = EventHandle.Create(typeof(T));
            EventCallbacks<T>.Value.Add(handle, eventCallback);

            if (!_events.TryGetValue(typeof(T), out Event e))
            {
                e = Event.Create<T>();
                _events.Add(typeof(T), e);
            }

            e.Handles.Add(handle);

            return handle;
        }
    }
}
