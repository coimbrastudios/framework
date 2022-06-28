using JetBrains.Annotations;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Creates a min and max slider for a field. The target field requires to have at least two serialized fields:
    /// <para>
    /// - One for the min value named as "x" or "_min".<br/>
    /// - One for the max value named as "y" or "_max".
    /// </para>
    /// </summary>
    public sealed class RangeSliderAttribute : ValidateAttribute
    {
        public readonly float MaxLimit;

        public readonly float MinLimit;

        public RangeSliderAttribute(float minLimit, float maxLimit, bool delayed = true)
            : base(delayed)
        {
            Debug.Assert(minLimit <= maxLimit, $"{nameof(minLimit)} should be smaller or equal to {nameof(maxLimit)}.");
            MinLimit = minLimit;
            MaxLimit = maxLimit;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the input will always be rounded to the closest integer.
        /// </summary>
        [PublicAPI]
        public bool RoundToInt { get; set; }
    }
}
