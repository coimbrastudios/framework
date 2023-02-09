namespace Coimbra.Services.Events
{
    /// <summary>
    /// Delegate for listening events.
    /// </summary>
    /// <param name="context">Contains useful information about the current invocation context.</param>
    /// <param name="e">The actual event data. Passed by reference to properly support large event structs.</param>
    /// <typeparam name="T">The event type.</typeparam>
    /// <seealso cref="IEvent"/>
    /// <seealso cref="IEventService"/>
    /// <seealso cref="EventContext"/>
    public delegate void EventContextHandler<T>(ref EventContext context, in T e)
        where T : IEvent;
}
