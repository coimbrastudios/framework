namespace Coimbra
{
    /// <summary>
    /// Forces an int or float to be equal or smaller than a value.
    /// </summary>
    public sealed class NotGreaterThanAttribute : ValidateAttribute
    {
        public readonly float Value;

        public NotGreaterThanAttribute(float value, bool delayed = true)
            : base(delayed)
        {
            Value = value;
        }
    }
}
