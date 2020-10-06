using JetBrains.Annotations;

namespace Coimbra
{
    /// <summary>
    ///     Create a reference from any value type.
    /// </summary>
    public sealed class ValueReference<T>
        where T : struct
    {
        public T Value;

        public ValueReference()
            : this(default) { }

        public ValueReference(T value)
        {
            Value = value;
        }

        [CanBeNull] [Pure]
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
