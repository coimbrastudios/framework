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

        private static class EventCallbacks<TEvent>
            where TEvent : IEvent, new()
        {
            internal static readonly Dictionary<EventHandle, EventData<TEvent>.Handler> Value = new(1);

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
            internal static Event Create<TEvent>(EventSystem eventSystem)
                where TEvent : IEvent, new()
            {
                return new Event(eventSystem, typeof(TEvent), EventCallbacks<TEvent>.RemoveHandler);
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

        /// <inheritdoc/>
        public EventHandle AddListener<TEvent>(EventData<TEvent>.Handler eventCallback)
            where TEvent : IEvent, new()
        {
            return AddListener(ref eventCallback);
        }

        /// <inheritdoc/>
        public bool AddListener<TEvent>(EventData<TEvent>.Handler eventCallback, List<EventHandle> appendList)
            where TEvent : IEvent, new()
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
        public bool CompareEventKey<TEvent>(EventKey eventKey)
            where TEvent : IEvent, new()
        {
            return _events.TryGetValue(typeof(TEvent), out Event e) ? e.Key == eventKey : eventKey == null;
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
        public bool HasAnyListeners<TEvent>()
            where TEvent : IEvent, new()
        {
            return _events.TryGetValue(typeof(TEvent), out Event e) && e.Count > 0;
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
        public bool IsInvoking<TEvent>()
            where TEvent : IEvent, new()
        {
            return _events.TryGetValue(typeof(TEvent), out Event e) && e.IsInvoking;
        }

        /// <inheritdoc/>
        public bool IsInvoking(Type eventType)
        {
            return _events.TryGetValue(eventType, out Event e) && e.IsInvoking;
        }

        /// <inheritdoc/>
        public bool Invoke<TEvent>(object eventSender, EventKey eventKey = null)
            where TEvent : IEvent, new()
        {
            EventData<TEvent> eventData = new(eventSender);

            return Invoke(_events, ref eventData, eventKey);
        }

        /// <inheritdoc/>
        public bool Invoke<TEvent>(object eventSender, TEvent eventValue, EventKey eventKey = null)
            where TEvent : IEvent, new()
        {
            EventData<TEvent> eventData = new(eventSender, ref eventValue);

            return Invoke(_events, ref eventData, eventKey);
        }

        /// <inheritdoc/>
        public bool Invoke<TEvent>(object eventSender, ref TEvent eventValue, EventKey eventKey = null)
            where TEvent : IEvent, new()
        {
            EventData<TEvent> eventData = new(eventSender, ref eventValue);

            return Invoke(_events, ref eventData, eventKey);
        }

        /// <inheritdoc/>
        public bool Invoke<TEvent>(EventData<TEvent> eventData, EventKey eventKey = null)
            where TEvent : IEvent, new()
        {
            return Invoke(_events, ref eventData, eventKey);
        }

        /// <inheritdoc/>
        public bool Invoke<TEvent>(ref EventData<TEvent> eventData, EventKey eventKey = null)
            where TEvent : IEvent, new()
        {
            return Invoke(_events, ref eventData, eventKey);
        }

        /// <inheritdoc/>
        public bool RemoveAllListeners<TEvent>(EventKey eventKey = null)
            where TEvent : IEvent, new()
        {
            if (!_events.TryGetValue(typeof(TEvent), out Event e))
            {
                return false;
            }

            if (e.Key == null || e.Key == eventKey || (e.Key.Restrictions & EventKey.RestrictionOptions.DisallowRemoveAll) == 0)
            {
                return e.RemoveAllListeners();
            }

            Debug.LogErrorFormat(InvalidEventKeyMessageFormat, eventKey, e.Key, typeof(TEvent));

            return false;
        }

        /// <inheritdoc/>
        public bool RemoveAllListeners(Type eventType, EventKey eventKey = null)
        {
            eventType.AssertNonInterfaceImplements<IEvent>();

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
        public bool ResetEventKey<TEvent>(EventKey eventKey)
            where TEvent : IEvent, new()
        {
            if (eventKey == null || !_events.TryGetValue(typeof(TEvent), out Event e))
            {
                return false;
            }

            if (e.Key != null && e.Key != eventKey)
            {
                Debug.LogErrorFormat(InvalidEventKeyMessageFormat, eventKey, e.Key, typeof(TEvent));

                return false;
            }

            e.Key = null;

            return true;
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
        public bool SetEventKey<TEvent>(EventKey eventKey)
            where TEvent : IEvent, new()
        {
            if (_events.TryGetValue(typeof(TEvent), out Event e))
            {
                if (e.Key != null && e.Key != eventKey)
                {
                    Debug.LogErrorFormat(InvalidEventKeyMessageFormat, eventKey, e.Key, typeof(TEvent));

                    return false;
                }
            }
            else
            {
                e = Event.Create<TEvent>(this);
                _events.Add(typeof(TEvent), e);
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
        private static bool Invoke<TEvent>(IReadOnlyDictionary<Type, Event> events, ref EventData<TEvent> eventData, EventKey eventKey)
            where TEvent : IEvent, new()
        {
            if (!events.TryGetValue(typeof(TEvent), out Event e))
            {
                return false;
            }

            if (e.Key != null && e.Key != eventKey && (e.Key.Restrictions & EventKey.RestrictionOptions.DisallowInvoke) != 0)
            {
                Debug.LogErrorFormat(InvalidEventKeyMessageFormat, eventKey, e.Key, typeof(TEvent));

                return false;
            }

            if (e.IsInvoking)
            {
                Debug.LogError($"{typeof(TEvent)} is already being invoked! Skipping its invocation to avoid a stack overflow.");

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
                        EventCallbacks<TEvent>.Value[eventData.CurrentHandle].Invoke(ref eventData);
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(new Exception($"An exception occurred while invoking {typeof(TEvent)}!", exception));
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
        private EventHandle AddListener<TEvent>(ref EventData<TEvent>.Handler eventCallback)
            where TEvent : IEvent, new()
        {
            if (eventCallback == null)
            {
                return default;
            }

            EventHandle handle = EventHandle.Create(typeof(TEvent));
            EventCallbacks<TEvent>.Value.Add(handle, eventCallback);

            if (!_events.TryGetValue(typeof(TEvent), out Event e))
            {
                e = Event.Create<TEvent>(this);
                _events.Add(typeof(TEvent), e);
            }

            e.Add(in handle);

            return handle;
        }
    }
}
