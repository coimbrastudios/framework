namespace Coimbra
{
    /// <summary>
    /// Draws a field as if it was an <see cref="IntRange"/>. The target field requires to have at least two serialized fields:
    /// <para>
    /// - "x" or "_min".<br/>
    /// - "y" or "_max".
    /// </para>
    /// </summary>
    public sealed class IntRangeAttribute : ValidateAttribute
    {
        public IntRangeAttribute(bool delayed = true)
            : base(delayed) { }
    }
}
