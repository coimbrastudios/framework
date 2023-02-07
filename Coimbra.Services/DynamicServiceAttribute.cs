using JetBrains.Annotations;
using System;
using UnityEngine.Scripting;

namespace Coimbra.Services
{
    /// <summary>
    /// Add this attribute to an <see cref="IService"/> definition interface to enable its value to change at runtime.
    /// </summary>
    /// <remarks>
    /// By default, once a <see cref="IService"/> is set it can't be overriden until <see cref="ServiceLocator.IsSet{T}()"/> returns false again. You can disable that by using that attribute in your definition interface.
    /// <para></para>
    /// This can be useful for per-scene <see cref="IService"/> that can have the <see cref="ServiceLocator.Set{T}"/> called before the previous instance was unloaded.
    /// </remarks>
    /// <seealso cref="IService"/>
    /// <seealso cref="IServiceFactory"/>
    /// <seealso cref="ServiceLocator"/>
    [AttributeUsage(AttributeTargets.Interface)]
    [BaseTypeRequired(typeof(IService))]
    [Preserve]
    public sealed class DynamicServiceAttribute : Attribute { }
}
