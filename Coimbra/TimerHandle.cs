using System;
using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Handle for a timer from <see cref="ITimerService"/>.
    /// </summary>
    [Preserve]
    public readonly struct TimerHandle : IEquatable<TimerHandle>
    {
        public readonly Guid Guid;

        private TimerHandle(Guid guid)
        {
            Guid = guid;
        }

        public bool IsValid => Guid != Guid.Empty;

        public static TimerHandle Create()
        {
            return new TimerHandle(Guid.NewGuid());
        }

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
