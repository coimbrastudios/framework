using JetBrains.Annotations;
using System;
using UnityEngine.Scripting;

namespace Coimbra.Services.Events
{
    /// <summary>
    /// An event being invoked.
    /// </summary>
    [Preserve]
    public ref struct EventContext
    {
        /// <summary>
        /// The <see cref="IEventService"/> used to invoke the event.
        /// </summary>
        [NotNull]
        public readonly IEventService Service;

        /// <summary>
        /// The object that requested the event invocation.
        /// </summary>
        [NotNull]
        public readonly object Sender;

        /// <summary>
        /// The event type.
        /// </summary>
        [NotNull]
        public readonly Type Type;

        /// <summary>
        /// The handle for the current call.
        /// </summary>
        public EventHandle CurrentHandle;

        public EventContext([NotNull] IEventService service, [NotNull] object sender, [NotNull] Type type)
        {
            Service = service;
            Sender = sender;
            Type = type;
            CurrentHandle = default;
        }
    }
}
