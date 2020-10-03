using Coimbra.Attributes;
using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Coimbra
{
    /// <summary>
    ///     Stores a range between 2 floats.
    /// </summary>
    [Serializable]
    public struct FloatRange : IEquatable<FloatRange>
    {
        [FormerlySerializedAs("m_Min")] [Summary(nameof(Min))]
        [SerializeField] private float _min;
        [FormerlySerializedAs("m_Max")] [Summary(nameof(Max))]
        [SerializeField] private float _max;

        /// <param name="a"> The <see cref="Min"/> or <see cref="Max"/> value. </param>
        /// <param name="b"> The <see cref="Min"/> or <see cref="Max"/> value. </param>
        [PublicAPI]
        public FloatRange(float a, float b = 0)
        {
            _min = a < b ? a : b;
            _max = a > b ? a : b;
        }

        /// <summary>
        ///     The diff between <see cref="Max"/> and <see cref="Min"/>.
        /// </summary>
        [PublicAPI]
        public float Lenght => Max - Min;

        /// <summary>
        ///     The biggest value on the range.
        /// </summary>
        [PublicAPI]
        public float Max => _max;

        /// <summary>
        ///     The smallest value on the range.
        /// </summary>
        [PublicAPI]
        public float Min => _min;

        /// <summary>
        ///     Returns a random float number between <see cref="Min"/> [inclusive] and <see cref="Max"/> [inclusive].
        /// </summary>
        [PublicAPI]
        public float Random => UnityEngine.Random.Range(Min, Max);

        /// <summary>
        ///     The sum of <see cref="Min"/> and <see cref="Max"/>.
        /// </summary>
        [PublicAPI]
        public float Sum => Min + Max;

        [PublicAPI] [Pure]
        public static implicit operator Vector2(FloatRange value)
        {
            return new Vector2(value.Min, value.Max);
        }

        [PublicAPI] [Pure]
        public static implicit operator FloatRange(Vector2 value)
        {
            return new FloatRange(value.x, value.y);
        }

        [PublicAPI] [Pure]
        public static bool operator ==(FloatRange a, FloatRange b)
        {
            return Mathf.Approximately(a.Min, b.Min) && Mathf.Approximately(a.Max, b.Max);
        }

        [PublicAPI] [Pure]
        public static bool operator !=(FloatRange a, FloatRange b)
        {
            return !(a == b);
        }

        /// <summary>
        ///     Returns true if the value is between min [inclusive] and max [inclusive].
        /// </summary>
        [PublicAPI] [Pure]
        public bool Contains(int value)
        {
            return value >= Min && value <= Max;
        }

        /// <summary>
        ///     Returns true if the value is between min [inclusive] and max [inclusive].
        /// </summary>
        [PublicAPI] [Pure]
        public bool Contains(float value)
        {
            return value >= Min && value <= Max;
        }

        [PublicAPI] [Pure]
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

        [PublicAPI] [Pure]
        public override int GetHashCode()
        {
            return (Min.GetHashCode() + Max.GetHashCode()) * 37;
        }

        [NotNull] [PublicAPI] [Pure]
        public override string ToString()
        {
            return $"[{Min:F}, {Max:F}]";
        }

        [PublicAPI] [Pure]
        public bool Equals(FloatRange other)
        {
            return this == other;
        }
    }
}
