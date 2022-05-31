namespace Coimbra
{
    /// <summary>
    /// Forces an int or float to be equal or greater than a value.
    /// </summary>
    public sealed class NotLessThanAttribute : ValidateAttribute
    {
        public readonly float Value;

        public NotLessThanAttribute(float value, bool delayed = true)
            : base(delayed)
        {
            Value = value;
        }
    }
}
