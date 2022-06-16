#nullable enable

using System;

namespace Coimbra.Services.Events
{
    /// <summary>
    /// Delegate for listening when an event type starts/stops being relevant.
    /// </summary>
    public delegate void EventRelevancyChangedHandler(IEventService service, Type type, bool isRelevant);
}
