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
        internal static readonly Dictionary<EventHandle, EventContextHandler<TEvent>> Value = new Dictionary<EventHandle, EventContextHandler<TEvent>>(1);

        internal static readonly Func<EventHandle, bool> RemoveHandler = Value.Remove;
    }
}
