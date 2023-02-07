using System;

namespace Coimbra
{
    /// <summary>
    /// Add to a <see cref="SerializableDictionary{TKey,TValue}"/> or <see cref="SerializableTypeDictionary{TKey,TValue,TFilter}"/> to hide the key label.
    /// </summary>
    /// <seealso cref="SerializableDictionary{TKey,TValue}"/>
    /// <seealso cref="SerializableTypeDictionary{TKey,TValue,TFilter}"/>
    /// <seealso cref="DisableResizeAttribute"/>
    /// <seealso cref="HideValueLabelAttribute"/>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class HideKeyLabelAttribute : Attribute { }
}
