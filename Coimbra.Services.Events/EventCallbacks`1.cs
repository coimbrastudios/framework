#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Coimbra.Services.Events
{
    [Preserve]
    internal static class EventCallbacks<TEvent>
        where TEvent : IEvent
    {
        internal static readonly Dictionary<EventHandle, EventContextHandler<TEvent>> Value = new(1);

        internal static readonly Func<EventHandle, bool> RemoveHandler = Value.Remove;

        internal static readonly Func<EventHandle, List<DelegateListener>, int> GetListenersHandler = delegate(EventHandle handle, List<DelegateListener> list)
        {
            return Value.TryGetValue(handle, out EventContextHandler<TEvent> handler) ? handler.GetListeners(list) : 0;
        };
    }
}
