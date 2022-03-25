namespace Coimbra
{
    /// <summary>
    /// The event key to be used in <see cref="IEventService"/> APIs.
    /// </summary>
    public sealed class EventKey
    {
        /// <summary>
        /// The restrictions being applied.
        /// </summary>
        public readonly EventKeyRestrictions Restrictions;

        public EventKey(EventKeyRestrictions restrictions = EventKeyRestrictions.All)
        {
            Restrictions = restrictions;
        }
    }
}
