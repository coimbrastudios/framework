namespace Coimbra.Services
{
    /// <summary>
    /// A reference to an event being invoked.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public ref struct EventRef<T>
        where T : IEvent
    {
        /// <summary>
        /// Generic delegate for listening events from the <see cref="IEventService"/>.
        /// </summary>
        public delegate void Handler(ref EventRef<T> e);

        /// <summary>
        /// The one that invoked the event.
        /// </summary>
        public readonly object Sender;

        /// <summary>
        /// The actual value of the event.
        /// </summary>
        public readonly T Value;

        /// <summary>
        /// The handle for the current call.
        /// </summary>
        public EventHandle Handle;

        public EventRef(object sender)
        {
            Sender = sender;
            Value = default;
            Handle = default;
        }

        public EventRef(object sender, ref T value)
        {
            Sender = sender;
            Value = value;
            Handle = default;
        }
    }
}
