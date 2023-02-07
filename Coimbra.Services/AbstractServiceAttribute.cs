using JetBrains.Annotations;
using System;

namespace Coimbra.Services
{
    /// <summary>
    /// Add this attribute to an <see cref="IService"/> definition interface to emit an error when trying to use it with <see cref="ServiceLocator"/> APIs directly.
    /// </summary>
    /// <remarks>
    /// By default, any interface implementing <see cref="IService"/> is considered a valid service type. Use this attribute to create interfaces that should not be considered a valid service type for <see cref="ServiceLocator"/> APIs.
    /// <para></para>
    /// Used when it makes sense to have a common interface between different <see cref="IService"/> without allowing it to be used with the <see cref="ServiceLocator"/> directly.
    /// <para></para>
    /// The best example is <see cref="IService"/> itself which is required to be implemented by any service, ensuring that <see cref="ServiceLocator"/> only allows types designed to be used with it.
    /// </remarks>
    /// <seealso cref="IService"/>
    /// <seealso cref="IServiceFactory"/>
    /// <seealso cref="ServiceLocator"/>
    [AttributeUsage(AttributeTargets.Interface)]
    [BaseTypeRequired(typeof(IService))]
    public sealed class AbstractServiceAttribute : Attribute { }
}
