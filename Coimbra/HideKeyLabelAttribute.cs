using System;

namespace Coimbra
{
    /// <summary>
    /// Add to a <see cref="SerializableDictionary{TKey,TValue}"/> to hide the key label.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class HideKeyLabelAttribute : Attribute { }
}
