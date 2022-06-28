#nullable enable

using JetBrains.Annotations;
using System;

namespace Coimbra
{
    /// <summary>
    /// Use it on a static partial class to generate a global <see cref="ManagedPool{T}"/>.
    /// <seealso cref="ManagedPool"/>
    /// <seealso cref="DictionaryPool"/>
    /// <seealso cref="HashSetPool"/>
    /// <seealso cref="ListPool"/>
    /// <seealso cref="QueuePool"/>
    /// <seealso cref="StackPool"/>
    /// <seealso cref="StringBuilderPool"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class SharedManagedPoolAttribute : Attribute
    {
        [UsedImplicitly]
        public readonly string InstanceValueField;

        [UsedImplicitly]
        public readonly string? NestedInstanceWrapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharedManagedPoolAttribute"/> class.
        /// </summary>
        /// <param name="instanceValueField">The name of the static field containing the <see cref="ManagedPool{T}"/> instance value.</param>
        /// <param name="nestedInstanceWrapper">The nested static class name wrapping the <paramref name="instanceValueField"/>, if any.</param>
        public SharedManagedPoolAttribute(string instanceValueField, string? nestedInstanceWrapper = null)
        {
            InstanceValueField = instanceValueField;
            NestedInstanceWrapper = nestedInstanceWrapper;
        }
    }
}
