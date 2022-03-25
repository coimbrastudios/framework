namespace Coimbra
{
    /// <summary>
    /// Generic delegate for listening events from the <see cref="IEventService"/>.
    /// </summary>
    public delegate void EventListenerHandler<T>(object sender, ref T e);
}
