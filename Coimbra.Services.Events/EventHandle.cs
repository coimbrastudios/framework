#nullable enable

using System;
using UnityEngine.Scripting;

namespace Coimbra.Services.Events
{
    /// <summary>
    /// Handle for an event from <see cref="IEventService"/>.
    /// </summary>
    [Preserve]
    [Serializable]
    public readonly struct EventHandle : IEquatable<EventHandle>
    {
        public readonly IEventService Service;

        public readonly Guid Guid;

        public readonly Type? Type;

        private EventHandle(IEventService service, Guid guid, Type type)
        {
            Service = service;
            Guid = guid;
            Type = type;
        }

        public bool IsValid => Guid != Guid.Empty && Service != null && Service.HasListener(in this);

        public static bool operator ==(EventHandle left, EventHandle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EventHandle left, EventHandle right)
        {
            return !left.Equals(right);
        }

        public static EventHandle Create(IEventService service, Type type)
        {
            return new EventHandle(service, Guid.NewGuid(), type);
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
