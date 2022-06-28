using JetBrains.Annotations;
using System;

namespace Coimbra.Services
{
    /// <summary>
    /// Apply this on a concrete <see cref="IService"/> implementation to disable the default factory for it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [BaseTypeRequired(typeof(IService))]
    public sealed class DisableDefaultFactoryAttribute : Attribute { }
}
