using System;
using UnityEngine.Scripting;

namespace Coimbra
{
    [Preserve]
    [Serializable]
    public readonly struct TimerHandle : IEquatable<TimerHandle>
    {
        public readonly Guid Guid;

        public TimerHandle(Guid guid)
        {
            Guid = guid;
        }

        public bool IsValid => Guid != Guid.Empty;

        public static bool operator ==(TimerHandle left, TimerHandle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TimerHandle left, TimerHandle right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            return obj is TimerHandle other && Equals(other);
        }

        public bool Equals(TimerHandle other)
        {
            return Guid.Equals(other.Guid);
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }
    }
}
