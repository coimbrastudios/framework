using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Coimbra
{
    /// <summary>
    /// Stores a range between 2 ints.
    /// </summary>
    [Preserve]
    [Serializable]
    public struct IntRange : IEquatable<IntRange>
    {
        [FormerlySerializedAs("x")]
        [FormerlySerializedAs("m_X")]
        [FormerlySerializedAs("m_Min")]
        [Tooltip("The smallest value on the range.")]
        [SerializeField]
        private int _min;

        [FormerlySerializedAs("y")]
        [FormerlySerializedAs("m_Y")]
        [FormerlySerializedAs("m_Max")]
        [Tooltip("The biggest value on the range.")]
        [SerializeField]
        private int _max;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntRange"/> struct.
        /// </summary>
        public IntRange(int a, int b = 0)
        {
            _min = a < b ? a : b;
            _max = a > b ? a : b;
        }

        /// <summary>
        /// Gets the diff between <see cref="Max"/> and <see cref="Min"/>.
        /// </summary>
        public int Lenght => Max - Min;

        /// <summary>
        /// Gets the biggest value on the range.
        /// </summary>
        public int Max => _max;

        /// <summary>
        /// Gets the smallest value on the range.
        /// </summary>
        public int Min => _min;

        /// <summary>
        /// Gets a random integer number between <see cref="Min"/> [inclusive] and <see cref="Max"/> [exclusive].
        /// </summary>
        public int RandomExclusive => Random.Range(Min, Max);

        /// <summary>
        /// Gets a random integer number between <see cref="Min"/> [inclusive] and <see cref="Max"/> [inclusive].
        /// </summary>
        public int RandomInclusive => Random.Range(Min, Max + 1);

        [Pure]
        public static implicit operator IntRange(int value)
        {
            return new IntRange(value, value);
        }

        [Pure]
        public static implicit operator FloatRange(IntRange value)
        {
            return new FloatRange(value.Min, value.Max);
        }

        [Pure]
        public static implicit operator Vector2(IntRange value)
        {
            return new Vector2(value.Min, value.Max);
        }

        [Pure]
        public static implicit operator Vector2Int(IntRange value)
        {
            return new Vector2Int(value.Min, value.Max);
        }

        [Pure]
        public static implicit operator IntRange(Vector2Int value)
        {
            return new IntRange(value.x, value.y);
        }

        [Pure]
        public static bool operator ==(IntRange a, IntRange b)
        {
            return a.Min == b.Min && a.Max == b.Max;
        }

        [Pure]
        public static bool operator !=(IntRange a, IntRange b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Returns true if the value is between min [inclusive] and max [exclusive].
        /// </summary>
        [Pure]
        public bool ContainsExclusive(int value)
        {
            return value >= Min && value < Max;
        }

        /// <summary>
        /// Returns true if the value is between min [inclusive] and max [exclusive].
        /// </summary>
        [Pure]
        public bool ContainsExclusive(float value)
        {
            return value >= Min && value < Max;
        }

        /// <summary>
        /// Returns true if the value is between min [inclusive] and max [inclusive].
        /// </summary>
        [Pure]
        public bool ContainsInclusive(int value)
        {
            return value >= Min && value <= Max;
        }

        /// <summary>
        /// Returns true if the value is between min [inclusive] and max [inclusive].
        /// </summary>
        [Pure]
        public bool ContainsInclusive(float value)
        {
            return value >= Min && value <= Max;
        }

        [Pure]
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (base.Equals(obj))
            {
                return true;
            }

            switch (obj)
            {
                case IntRange _:
                case Vector2Int _:

                    return this == (IntRange)obj;

                case FloatRange _:
                case Vector2 _:

                    return this == (FloatRange)obj;

                default:

                    return false;
            }
        }

        [Pure]
        public override int GetHashCode()
        {
#if UNITY_2021_3_OR_NEWER
            return HashCode.Combine(Min, Max);
#else
            return ((Vector2Int)this).GetHashCode();
#endif
        }

        [NotNull]
        [Pure]
        public override string ToString()
        {
            return $"[{Min}, {Max}]";
        }

        [Pure]
        public bool Equals(IntRange other)
        {
            return this == other;
        }
    }
}
