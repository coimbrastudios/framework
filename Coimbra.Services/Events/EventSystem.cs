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
        private delegate bool RemoveHandler(EventHandle key);

        private static class EventCallbacks<T>
            where T : IEvent
        {
            internal static readonly Dictionary<EventHandle, EventData<T>.Handler> Value = new(1);

            internal static readonly RemoveHandler RemoveHandler = Value.Remove;
        }

        private sealed class Event
        {
            internal readonly HashSet<EventHandle> HandlesToRemove = new();

            internal bool IsInvoking;

            internal EventKey Key;

            private readonly EventSystem _eventSystem;

            private readonly Type _type;

            private readonly RemoveHandler _removeCallbackHandler;

            private readonly List<EventHandle> _handles = new();

            private Event(EventSystem eventSystem, Type type, RemoveHandler removeCallbackHandler)
            {
                _eventSystem = eventSystem;
                _type = type;
                _removeCallbackHandler = removeCallbackHandler;
            }

            internal EventHandle this[int index] => _handles[index];

            internal int Count => _handles.Count;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static Event Create<T>(EventSystem eventSystem)
                where T : IEvent
            {
                return new Event(eventSystem, typeof(T), EventCallbacks<T>.RemoveHandler);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal void Add(in EventHandle handle)
            {
                _handles.Add(handle);

                if (_handles.Count == 1)
                {
                    _eventSystem.OnFirstListenerAdded?.Invoke(_eventSystem, _type);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal bool Contains(in EventHandle handle)
            {
                return _handles.Contains(handle);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal bool Remove(in EventHandle handle)
            {
                if (!_removeCallbackHandler.Invoke(handle))
                {
                    return false;
                }

                if (_handles.Remove(handle) && _handles.Count == 0)
                {
                    _eventSystem.OnLastListenerRemoved?.Invoke(_eventSystem, _type);
                }

                return true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal bool RemoveAllListeners()
            {
                bool result = false;

                if (IsInvoking)
                {
                    foreach (EventHandle handle in _handles)
                    {
                        result |= HandlesToRemove.Add(handle);
                    }
                }
                else
                {
                    foreach (EventHandle handle in _handles)
                    {
                        result |= _removeCallbackHandler.Invoke(handle);
                    }

                    _handles.Clear();

                    if (result)
                    {
                        _eventSystem.OnLastListenerRemoved?.Invoke(_eventSystem, _type);
                    }
                }

                return result;
            }
        }

        /// <inheritdoc/>
        public event IEventService.EventHandler OnFirstListenerAdded;

        /// <inheritdoc/>
        public event IEventService.EventHandler OnLastListenerRemoved;

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
            eventType.AssertNonInterfaceImplements<IEvent>();

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
            where T : IEvent, new()
        {
            EventData<T> eventData = new(eventSender);

            return Invoke(_events, ref eventData, eventKey);
        }

        /// <inheritdoc/>
        public bool Invoke<T>(object eventSender, T eventValue, EventKey eventKey = null)
            where T : IEvent
        {
            EventData<T> eventData = new(eventSender, ref eventValue);

            return Invoke(_events, ref eventData, eventKey);
        }

        /// <inheritdoc/>
        public bool Invoke<T>(object eventSender, ref T eventValue, EventKey eventKey = null)
            where T : IEvent
        {
            EventData<T> eventData = new(eventSender, ref eventValue);

            return Invoke(_events, ref eventData, eventKey);
        }

        /// <inheritdoc/>
        public bool Invoke<T>(EventData<T> eventData, EventKey eventKey = null)
            where T : IEvent
        {
            return Invoke(_events, ref eventData, eventKey);
        }

        /// <inheritdoc/>
        public bool Invoke<T>(ref EventData<T> eventData, EventKey eventKey = null)
            where T : IEvent
        {
            return Invoke(_events, ref eventData, eventKey);
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

            return e.RemoveAllListeners();
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
            if (!eventHandle.IsValid || !_events.TryGetValue(eventHandle.Type, out Event e))
            {
                return false;
            }

            return e.IsInvoking ? e.HandlesToRemove.Add(eventHandle) : e.Remove(in eventHandle);
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
                e = Event.Create<T>(this);
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
        private static bool Invoke<T>(IReadOnlyDictionary<Type, Event> events, ref EventData<T> eventData, EventKey eventKey)
            where T : IEvent
        {
            typeof(T).AssertNonInterfaceImplements<IEvent>();

            if (!events.TryGetValue(typeof(T), out Event e))
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

            e.IsInvoking = true;

            try
            {
                for (int i = 0; i < count; i++)
                {
                    eventData.CurrentHandle = e[i];

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
                e.Remove(in handle);
            }

            e.HandlesToRemove.Clear();

            return true;
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
                e = Event.Create<T>(this);
                _events.Add(typeof(T), e);
            }

            e.Add(in handle);

            return handle;
        }
    }
}
