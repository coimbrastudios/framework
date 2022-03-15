using System;
using UnityEngine.Scripting;

namespace Coimbra
{
    [Preserve]
    [Serializable]
    public struct TimerHandle : IEquatable<TimerHandle>
    {
        public Guid Guid { get; private set; }

        /// <summary>
        /// Expected to be called from within a <see cref="ITimerService"/>.
        /// </summary>
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

        /// <summary>
        /// Expected to be called from within a <see cref="ITimerService"/>.
        /// </summary>
        public void Initialize(Guid guid)
        {
            Guid = guid;
        }

        /// <summary>
        /// Expected to be called from within a <see cref="ITimerService"/>.
        /// </summary>
        public void Invalidate()
        {
            Guid = Guid.Empty;
        }
    }
}
