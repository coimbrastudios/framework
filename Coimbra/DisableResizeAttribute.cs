using System;

namespace Coimbra
{
    /// <summary>
    /// Apply to a <see cref="SerializableDictionary{TKey,TValue}"/> or <see cref="SerializableTypeDictionary{TKey,TValue,TFilter}"/> to lock its size in the inspector.
    /// </summary>
    /// <seealso cref="SerializableDictionary{TKey,TValue}"/>
    /// <seealso cref="SerializableTypeDictionary{TKey,TValue,TFilter}"/>
    /// <seealso cref="HideKeyLabelAttribute"/>
    /// <seealso cref="HideValueLabelAttribute"/>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DisableResizeAttribute : Attribute { }
}
