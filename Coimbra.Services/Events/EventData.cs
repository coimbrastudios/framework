using UnityEngine.Scripting;

namespace Coimbra.Services.Events
{
    /// <summary>
    /// A reference to an event being invoked.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Preserve]
    public ref struct EventData<T>
        where T : IEvent
    {
        /// <summary>
        /// Generic delegate for listening events from the <see cref="IEventService"/>.
        /// </summary>
        public delegate void Handler(ref EventData<T> e);

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
        public EventHandle CurrentHandle;

        public EventData(object sender)
        {
            Sender = sender;
            Value = default;
            CurrentHandle = default;
        }

        public EventData(object sender, T value)
        {
            Sender = sender;
            Value = value;
            CurrentHandle = default;
        }

        public EventData(object sender, ref T value)
        {
            Sender = sender;
            Value = value;
            CurrentHandle = default;
        }
    }
}
