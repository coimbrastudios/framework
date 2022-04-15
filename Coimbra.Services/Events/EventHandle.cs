using System;
using UnityEngine.Scripting;

namespace Coimbra.Services.Events
{
    /// <summary>
    /// Handle for an event from <see cref="IEventService"/>.
    /// </summary>
    [Preserve]
    public readonly struct EventHandle : IEquatable<EventHandle>
    {
        public readonly Guid Guid;

        public readonly Type Type;

        private EventHandle(Guid guid, Type type)
        {
            Guid = guid;
            Type = type;
        }

        public bool IsValid => Guid != Guid.Empty && Type != null;

        public static EventHandle Create(Type type)
        {
            return new EventHandle(Guid.NewGuid(), type);
        }

        public static bool operator ==(EventHandle left, EventHandle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EventHandle left, EventHandle right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            return obj is EventHandle other && Equals(other);
        }

        public bool Equals(EventHandle other)
        {
            return Guid.Equals(other.Guid);
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }
    }
}
