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
        public int Id { get; internal set; }

        [field: SerializeField]
        public int Version { get; internal set; }

        internal TimerHandle(int id, int version = 1)
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

        public bool Equals(TimerHandle other)
        {
            return Id == other.Id && Version == other.Version;
        }

        public override bool Equals(object obj)
        {
            return obj is TimerHandle other && Equals(other);
        }

        public override int GetHashCode()
        {
            return IsValid ? Id ^ Version : Id.GetHashCode();
        }
    }
}
