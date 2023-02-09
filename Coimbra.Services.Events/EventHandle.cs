#nullable enable

using System;
using UnityEngine.Scripting;

namespace Coimbra.Services.Events
{
    /// <summary>
    /// Handle for an event from <see cref="IEventService"/>.
    /// </summary>
    /// <remarks>
    /// Used to remove a specific listener or retrieve debug information through an <see cref="IEventService"/>.
    /// <para></para>
    /// You can use an <see cref="EventHandleTrackerComponent"/> in any <see cref="Actor"/> to link and <see cref="EventHandle"/> to the listener's lifecycle.
    /// </remarks>
    /// <seealso cref="IEvent"/>
    /// <seealso cref="IEventService"/>
    /// <seealso cref="EventHandleTrackerComponent"/>
    [Preserve]
    [Serializable]
    public readonly struct EventHandle : IEquatable<EventHandle>
    {
        /// <summary>
        /// The <see cref="IEventService"/> that generated this <see cref="EventHandle"/>.
        /// </summary>
        public readonly IEventService Service;

        /// <summary>
        /// An unique id to identify this <see cref="EventHandle"/>.
        /// </summary>
        public readonly Guid Guid;

        /// <summary>
        /// The event type that this <see cref="EventHandle"/> refers to.
        /// </summary>
        public readonly Type? Type;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandle"/> struct. Only meant to be called from inside an <see cref="IEventService"/> implementation.
        /// </summary>
        public EventHandle(IEventService service, Type type)
        {
            Guid = Guid.NewGuid();
            Service = service;
            Type = type;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="EventHandle"/> was ever initialized and if the listener it refers to wasn't removed already from the <see cref="IEventService"/> that generated this <see cref="EventHandle"/>.
        /// </summary>
        public bool IsValid => Guid != Guid.Empty && Service != null && Service.HasListener(in this);

        public static bool operator ==(EventHandle left, EventHandle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EventHandle left, EventHandle right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is EventHandle other && Equals(other);
        }

        /// <inheritdoc/>
        public bool Equals(EventHandle other)
        {
            return Guid.Equals(other.Guid);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Guid.ToString();
        }
    }
}
