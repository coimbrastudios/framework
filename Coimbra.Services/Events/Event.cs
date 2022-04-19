using UnityEngine.Scripting;

namespace Coimbra.Services.Events
{
    /// <summary>
    /// An event being invoked.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Preserve]
    public ref struct Event<T>
        where T : IEvent
    {
        /// <summary>
        /// Generic delegate for listening events from the <see cref="IEventService"/>.
        /// </summary>
        public delegate void Handler(ref Event<T> e);

        /// <summary>
        /// The object that requested the event invocation.
        /// </summary>
        public readonly object Sender;

        /// <summary>
        /// The <see cref="IEventService"/> used to invoke the event.
        /// </summary>
        public readonly IEventService Service;

        /// <summary>
        /// The data of the event.
        /// </summary>
        public readonly T Data;

        /// <summary>
        /// The handle for the current call.
        /// </summary>
        public EventHandle CurrentHandle;

        public Event(IEventService service, object sender)
        {
            Service = service;
            Sender = sender;
            Data = default;
            CurrentHandle = default;
        }

        public Event(IEventService service, object sender, ref T data)
        {
            Service = service;
            Sender = sender;
            Data = data;
            CurrentHandle = default;
        }
    }
}
