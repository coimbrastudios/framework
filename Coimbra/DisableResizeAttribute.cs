using System;

namespace Coimbra
{
    /// <summary>
    /// Apply to a <see cref="SerializableDictionary{TKey,TValue}"/> to lock its size in the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DisableResizeAttribute : Attribute { }
}
