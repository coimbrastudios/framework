using JetBrains.Annotations;
using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Create a reference from any type.
    /// </summary>
    [Preserve]
    public sealed class Reference<T>
    {
        [CanBeNull]
        public T Value;

        public Reference([CanBeNull] T value)
        {
            Value = value;
        }

        [CanBeNull]
        [Pure]
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
