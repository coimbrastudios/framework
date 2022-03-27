namespace Coimbra
{
    /// <summary>
    /// Generic delegate for listening events from the <see cref="IEventService"/>.
    /// </summary>
    public delegate void EventListenerHandler<T>(ref EventRef<T> e)
        where T : IEvent;
}
