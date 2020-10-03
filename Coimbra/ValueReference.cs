using JetBrains.Annotations;

namespace Coimbra
{
    /// <summary>
    ///     Create a reference from any value type.
    /// </summary>
    public sealed class ValueReference<T>
        where T : struct
    {
        [PublicAPI]
        public T Value;

        [PublicAPI]
        public ValueReference()
            : this(default) { }

        [PublicAPI]
        public ValueReference(T value)
        {
            Value = value;
        }

        [CanBeNull] [PublicAPI] [Pure]
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
