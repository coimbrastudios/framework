using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Draws a field as if it was an <see cref="IntRange"/>. The target field requires to have at least two serialized fields:
    /// <para>
    /// - "x" or "_min"<br/>
    /// - "y" or "_max"
    /// </para>
    /// </summary>
    public sealed class IntRangeAttribute : PropertyAttribute
    {
        public readonly bool Delayed;

        public IntRangeAttribute(bool delayed = true)
        {
            Delayed = delayed;
        }
    }
}
