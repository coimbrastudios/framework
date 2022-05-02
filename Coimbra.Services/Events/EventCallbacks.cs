#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Coimbra.Services.Events
{
    [Preserve]
    internal static class EventCallbacks<TEvent>
        where TEvent : IEvent, new()
    {
        internal static readonly Dictionary<EventHandle, Event<TEvent>.Handler> Value = new(1);

        internal static readonly Func<EventHandle, bool> RemoveHandler = Value.Remove;
    }
}
