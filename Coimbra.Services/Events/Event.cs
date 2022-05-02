#nullable enable

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace Coimbra.Services.Events
{
    /// <summary>
    /// An event being invoked.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Preserve]
    public ref struct Event<T>
        where T : IEvent
    {
        /// <summary>
        /// Generic delegate for listening events from the <see cref="IEventService"/>.
        /// </summary>
        public delegate void Handler(ref Event<T> e);

        /// <summary>
        /// The <see cref="IEventService"/> used to invoke the event.
        /// </summary>
        public readonly IEventService Service;

        /// <summary>
        /// The object that requested the event invocation.
        /// </summary>
        public readonly object? Sender;

        /// <summary>
        /// The data of the event.
        /// </summary>
        public readonly T? Data;

        /// <summary>
        /// The handle for the current call.
        /// </summary>
        public EventHandle CurrentHandle;

        public Event(IEventService service, object? sender)
        {
            Service = service;
            Sender = sender;
            Data = default;
            CurrentHandle = default;
        }

        public Event(IEventService service, object? sender, ref T data)
        {
            Service = service;
            Sender = sender;
            Data = data;
            CurrentHandle = default;
        }
    }

    [Preserve]
    internal sealed class Event : IDisposable
    {
        internal readonly ref struct InvokeScope
        {
            private readonly Event _event;

            internal InvokeScope(Event e)
            {
                _event = e;
                _event.IsInvoking = true;
            }

            internal void Dispose()
            {
                _event.IsInvoking = false;

                foreach (EventHandle handle in _event._removeSet)
                {
                    _event.RemoveUnsafe(in handle);
                }

                _event._removeSet.Clear();
            }
        }

        internal event Action<Type>? OnFirstListenerAdded;

        internal event Action<Type>? OnLastListenerRemoved;

        internal EventKey? Key;

        private readonly Type _type;

        private readonly List<EventHandle> _handles = new();

        private readonly HashSet<EventHandle> _removeSet = new();

        private Func<EventHandle, bool> _removeCallbackHandler;

        private Event(Type type, Func<EventHandle, bool> removeCallbackHandler)
        {
            _type = type;
            _removeCallbackHandler = removeCallbackHandler;
        }

        internal EventHandle this[int index] => _handles[index];

        internal int Count => _handles.Count;

        internal bool IsInvoking { get; private set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Event Create<T>()
            where T : IEvent, new()
        {
            return new Event(typeof(T), EventCallbacks<T>.RemoveHandler);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Add(in EventHandle handle)
        {
            _handles.Add(handle);

            if (_handles.Count == 1)
            {
                OnFirstListenerAdded?.Invoke(_type);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool Contains(in EventHandle handle)
        {
            return _handles.Contains(handle) && !IsRemoving(in handle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool IsRemoving(in EventHandle handle)
        {
            return _removeSet.Contains(handle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool Remove(in EventHandle handle)
        {
            return IsInvoking ? _removeSet.Add(handle) : RemoveUnsafe(in handle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool RemoveAllListeners()
        {
            bool result = false;

            if (IsInvoking)
            {
                foreach (EventHandle handle in _handles)
                {
                    result |= _removeSet.Add(handle);
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
                    OnLastListenerRemoved?.Invoke(_type);
                }
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool RemoveUnsafe(in EventHandle handle)
        {
            if (!_removeCallbackHandler.Invoke(handle))
            {
                return false;
            }

            if (_handles.Remove(handle) && _handles.Count == 0)
            {
                OnLastListenerRemoved?.Invoke(_type);
            }

            return true;
        }

        void IDisposable.Dispose()
        {
            OnFirstListenerAdded = null;
            OnLastListenerRemoved = null;
            _removeCallbackHandler = null!;
        }
    }
}
