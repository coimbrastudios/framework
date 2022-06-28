using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Coimbra
{
    /// <summary>
    /// Stores a range between 2 floats.
    /// </summary>
    [Preserve]
    [Serializable]
    public struct FloatRange : IEquatable<FloatRange>
    {
        [FormerlySerializedAs("x")]
        [FormerlySerializedAs("m_X")]
        [FormerlySerializedAs("m_Min")]
        [Tooltip("The smallest value on the range.")]
        [SerializeField]
        private float _min;

        [FormerlySerializedAs("y")]
        [FormerlySerializedAs("m_Y")]
        [FormerlySerializedAs("m_Max")]
        [Tooltip("The biggest value on the range.")]
        [SerializeField]
        private float _max;

        /// <summary>
        /// Initializes a new instance of the <see cref="FloatRange"/> struct.
        /// </summary>
        public FloatRange(float a, float b = 0)
        {
            _min = a < b ? a : b;
            _max = a > b ? a : b;
        }

        /// <summary>
        /// Gets the diff between <see cref="Max"/> and <see cref="Min"/>.
        /// </summary>
        public float Lenght => Max - Min;

        /// <summary>
        /// Gets the biggest value on the range.
        /// </summary>
        public float Max => _max;

        /// <summary>
        /// Gets the smallest value on the range.
        /// </summary>
        public float Min => _min;

        /// <summary>
        /// Gets a random float number between <see cref="Min"/> [inclusive] and <see cref="Max"/> [inclusive].
        /// </summary>
        public float RandomValue => Random.Range(Min, Max);

        [Pure]
        public static implicit operator FloatRange(float value)
        {
            return new FloatRange(value, value);
        }

        [Pure]
        public static implicit operator Vector2(FloatRange value)
        {
            return new Vector2(value.Min, value.Max);
        }

        [Pure]
        public static implicit operator FloatRange(Vector2 value)
        {
            return new FloatRange(value.x, value.y);
        }

        [Pure]
        public static bool operator ==(FloatRange a, FloatRange b)
        {
            return Mathf.Approximately(a.Min, b.Min) && Mathf.Approximately(a.Max, b.Max);
        }

        [Pure]
        public static bool operator !=(FloatRange a, FloatRange b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Returns true if the value is between min [inclusive] and max [inclusive].
        /// </summary>
        [Pure]
        public bool ContainsValue(int value)
        {
            return value >= Min && value <= Max;
        }

        /// <summary>
        /// Returns true if the value is between min [inclusive] and max [inclusive].
        /// </summary>
        [Pure]
        public bool ContainsValue(float value)
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
            return ((Vector2)this).GetHashCode();
#endif
        }

        [NotNull]
        [Pure]
        public override string ToString()
        {
            return $"[{Min:F}, {Max:F}]";
        }

        [Pure]
        public bool Equals(FloatRange other)
        {
            return this == other;
        }
    }
}
