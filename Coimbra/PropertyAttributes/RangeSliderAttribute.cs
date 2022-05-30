using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Creates a min and max slider for a field. The target field requires to have at least two serialized fields:
    /// <para>
    /// - One for the min value named as "x" or "_min"<br/>
    /// - One for the max value named as "y" or "_max"
    /// </para>
    /// </summary>
    public sealed class RangeSliderAttribute : PropertyAttribute
    {
        public readonly float MaxLimit;

        public readonly float MinLimit;

        public readonly bool Delayed;

        public readonly bool RoundToInt;

        public RangeSliderAttribute(float minLimit, float maxLimit, bool roundToInt = false, bool delayed = true)
        {
            Debug.Assert(minLimit <= maxLimit, $"{nameof(minLimit)} should be smaller or equal to {nameof(maxLimit)}.");
            MinLimit = minLimit;
            MaxLimit = maxLimit;
            RoundToInt = roundToInt;
            Delayed = delayed;
        }
    }
}
