using JetBrains.Annotations;
using System;

namespace Coimbra.Services
{
    /// <summary>
    /// Apply this on a <see cref="IService"/> implementation class to disable the default factory for it.
    /// </summary>
    /// <remarks>
    /// By default, a factory is set for each new compatible type during <see cref="UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration"/>. You can disable that per-implementation class by using this attribute.
    /// <para></para>
    /// Add this in your <see cref="IService"/> implementation to stop its default <see cref="IServiceFactory"/>, if any, from being registered during <see cref="UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration"/>.
    /// </remarks>
    /// <seealso cref="DefaultServiceFactory{T}"/>
    /// <seealso cref="DefaultServiceActorFactory{T}"/>
    /// <seealso cref="IService"/>
    /// <seealso cref="IServiceFactory"/>
    /// <seealso cref="ServiceLocator"/>
    [AttributeUsage(AttributeTargets.Class)]
    [BaseTypeRequired(typeof(IService))]
    public sealed class DisableDefaultFactoryAttribute : Attribute { }
}
