using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace Coimbra.Services.Events
{
    [Preserve]
    [Serializable]
    internal sealed class Event : IDisposable
    {
        internal readonly ref struct InvokeScope
        {
            [NotNull]
            private readonly Event _event;

            internal InvokeScope([NotNull] Event e)
            {
                _event = e;
                _event.IsInvoking = true;
            }

            internal void Dispose()
            {
                _event.IsInvoking = false;

                foreach (EventHandle handle in _event._removeSet)
                {
                    _event.RemoveListenerUnsafe(in handle);
                }

                _event._removeSet.Clear();
            }
        }

        [CanBeNull]
        internal event EventRelevancyChangedHandler OnRelevancyChanged;

        [NotNull]
        internal readonly Func<EventHandle, List<DelegateListener>, int> GetListenersHandler;

        [NotNull]
        private readonly IEventService _service;

        [NotNull]
        private readonly Type _type;

        [NotNull]
        private readonly HashSet<EventHandle> _removeSet = new();

        [CanBeNull]
        [SerializeField]
        private string _label;

        [NotNull]
        [SerializeField]
        private List<EventHandle> _listeners = new();

        [NotNull]
        private Func<EventHandle, bool> _removeCallbackHandler;

        private Event([NotNull] IEventService service, [NotNull] Type type, [NotNull] Func<EventHandle, bool> removeCallbackHandler, [NotNull] Func<EventHandle, List<DelegateListener>, int> getListenersHandler)
        {
            _type = type;
            _service = service;
            _removeCallbackHandler = removeCallbackHandler;
            GetListenersHandler = getListenersHandler;
#if UNITY_EDITOR
            _label = TypeString.Get(_type);
#endif
        }

        internal EventHandle this[int index] => _listeners[index];

        internal bool IsInvoking { get; private set; }

        internal string Label => _label;

        internal int ListenerCount => _listeners.Count;

        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Event Create<T>([NotNull] IEventService eventService)
            where T : IEvent
        {
            return new Event(eventService, typeof(T), EventCallbacks<T>.RemoveHandler, EventCallbacks<T>.GetListenersHandler);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Add(in EventHandle handle)
        {
            _listeners.Add(handle);

            if (_listeners.Count == 1)
            {
                OnRelevancyChanged?.Invoke(_service, _type, true);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int GetListeners(List<DelegateListener> listeners)
        {
            int count = 0;

            foreach (EventHandle handle in _listeners)
            {
                count += GetListenersHandler.Invoke(handle, listeners);
            }

            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int GetRelevancyListeners(List<DelegateListener> listeners)
        {
            return OnRelevancyChanged?.GetListeners(listeners) ?? 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool HasListener(in EventHandle handle)
        {
            return _listeners.Contains(handle) && !IsRemoving(in handle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool IsRemoving(in EventHandle handle)
        {
            return _removeSet.Contains(handle);
        }

        internal bool RemoveAllListeners()
        {
            bool result = false;

            if (IsInvoking)
            {
                foreach (EventHandle handle in _listeners)
                {
                    result |= _removeSet.Add(handle);
                }
            }
            else
            {
                foreach (EventHandle handle in _listeners)
                {
                    result |= _removeCallbackHandler.Invoke(handle);
                }

                _listeners.Clear();

                if (result)
                {
                    OnRelevancyChanged?.Invoke(_service, _type, false);
                }
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool RemoveListener(in EventHandle handle)
        {
            return IsInvoking ? _removeSet.Add(handle) : RemoveListenerUnsafe(in handle);
        }

        private bool RemoveListenerUnsafe(in EventHandle handle)
        {
            if (!_removeCallbackHandler.Invoke(handle))
            {
                return false;
            }

            if (_listeners.Remove(handle) && _listeners.Count == 0)
            {
                OnRelevancyChanged?.Invoke(_service, _type, false);
            }

            return true;
        }

        void IDisposable.Dispose()
        {
            OnRelevancyChanged = null;
            _removeCallbackHandler = null!;
        }
    }
}
