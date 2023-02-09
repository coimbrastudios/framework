#nullable enable

using System;

namespace Coimbra.Services.Events
{
    /// <summary>
    /// Delegate for listening when an event type starts/stops being relevant.
    /// </summary>
    /// <seealso cref="IEvent"/>
    /// <seealso cref="IEventService"/>
    public delegate void EventRelevancyChangedHandler(IEventService service, Type type, bool isRelevant);
}
