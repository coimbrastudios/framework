using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace Coimbra
{
    [Preserve]
    [Serializable]
    public struct TimerHandle : IEquatable<TimerHandle>
    {
        [field: SerializeField]
        public int Id { get; private set; }

        [field: SerializeField]
        public int Version { get; private set; }

        /// <summary>
        /// Expected to be called from within a <see cref="ITimerService"/>.
        /// </summary>
        public TimerHandle(int id, int version = 1)
        {
            Id = id;
            Version = version;
        }

        public bool IsValid => Id != 0;

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
            return Id == other.Id && Version == other.Version;
        }

        public override int GetHashCode()
        {
            return IsValid ? Id ^ Version : Id.GetHashCode();
        }

        /// <summary>
        /// Expected to be called from within a <see cref="ITimerService"/>.
        /// </summary>
        public void Initialize(int id, int version)
        {
            Id = id;
            Version = version;
        }

        /// <summary>
        /// Expected to be called from within a <see cref="ITimerService"/>.
        /// </summary>
        public void Invalidate()
        {
            Id = 0;
        }
    }
}
