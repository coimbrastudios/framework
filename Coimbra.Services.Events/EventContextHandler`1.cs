namespace Coimbra.Services.Events
{
    /// <summary>
    /// Delegate for listening events.
    /// </summary>
    /// <typeparam name="T">The event type.</typeparam>
    public delegate void EventContextHandler<T>(ref EventContext context, in T e)
        where T : IEvent;
}
