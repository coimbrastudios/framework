using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Draws a field as if it was an <see cref="FloatRange"/>. The target field requires to have at least two serialized fields:
    /// <para>
    /// - "x" or "_min"<br/>
    /// - "y" or "_max"
    /// </para>
    /// </summary>
    public sealed class FloatRangeAttribute : PropertyAttribute
    {
        public readonly bool Delayed;

        public FloatRangeAttribute(bool delayed = true)
        {
            Delayed = delayed;
        }
    }
}
