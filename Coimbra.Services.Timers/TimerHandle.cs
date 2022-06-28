using System;
using UnityEngine.Scripting;

namespace Coimbra.Services.Timers
{
    /// <summary>
    /// Handle for a timer from <see cref="ITimerService"/>.
    /// </summary>
    [Preserve]
    [Serializable]
    public readonly struct TimerHandle : IEquatable<TimerHandle>
    {
        public readonly ITimerService Service;

        public readonly Guid Guid;

        private TimerHandle(ITimerService service, Guid guid)
        {
            Service = service;
            Guid = guid;
        }

        public bool IsValid => Guid != Guid.Empty && Service != null && Service.IsTimerActive(in this);

        public static bool operator ==(TimerHandle left, TimerHandle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TimerHandle left, TimerHandle right)
        {
            return !left.Equals(right);
        }

        public static TimerHandle Create(ITimerService service)
        {
            return new TimerHandle(service, Guid.NewGuid());
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is TimerHandle other && Equals(other);
        }

        /// <inheritdoc/>
        public bool Equals(TimerHandle other)
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
