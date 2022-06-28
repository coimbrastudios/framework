using JetBrains.Annotations;
using System;

namespace Coimbra
{
    /// <summary>
    /// Add to a partial class to make it have the same constructors as the base type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class CopyBaseConstructorsAttribute : Attribute
    {
        /// <summary>
        /// If true, only public constructors will be copied. If false, both public and protected constructors will be copied.
        /// </summary>
        [UsedImplicitly]
        public readonly bool IgnoreProtected;

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyBaseConstructorsAttribute"/> class.
        /// </summary>
        /// <param name="ignoreProtected">If true, only public constructors will be copied. If false, both public and protected constructors will be copied.</param>
        public CopyBaseConstructorsAttribute(bool ignoreProtected = false)
        {
            IgnoreProtected = ignoreProtected;
        }
    }
}
