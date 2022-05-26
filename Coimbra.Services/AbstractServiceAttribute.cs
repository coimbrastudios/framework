using JetBrains.Annotations;
using System;

namespace Coimbra.Services
{
    /// <summary>
    /// Add this attribute to an <see cref="IService"/> to emit an error when trying to use it with <see cref="ServiceLocator"/> APIs directly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    [BaseTypeRequired(typeof(IService))]
    public sealed class AbstractServiceAttribute : Attribute { }
}
