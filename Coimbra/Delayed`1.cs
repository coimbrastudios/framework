using System;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Wrapper for a value with <see cref="DelayedAttribute"/>.
    /// </summary>
    [Serializable]
    public struct Delayed<T>
    {
        /// <summary>
        /// The current value.
        /// </summary>
        [Delayed]
        public T Value;

        public static implicit operator T(Delayed<T> value)
        {
            return value.Value;
        }

        public static implicit operator Delayed<T>(T value)
        {
            return new Delayed<T>
            {
                Value = value,
            };
        }
    }
}
