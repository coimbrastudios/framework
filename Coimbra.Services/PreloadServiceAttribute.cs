using JetBrains.Annotations;
using System;
using UnityEngine;

namespace Coimbra.Services
{
    /// <summary>
    /// Apply this on a <see cref="IService"/> implementation class to preload the service during <see cref="RuntimeInitializeLoadType.BeforeSceneLoad"/>.
    /// </summary>
    /// <remarks>
    /// It will simply call <see cref="ServiceLocator.Get{T}"/> during <see cref="RuntimeInitializeLoadType.BeforeSceneLoad"/>.
    /// If the <see cref="IService"/> definition interface has <see cref="RequiredServiceAttribute"/> then it will call <see cref="ServiceLocator.GetChecked{T}"/> instead.
    /// <para></para>
    /// As it does nothing else you still need to have a <see cref="IServiceFactory"/> set for the given <see cref="IService"/> for this attribute to work as expected.
    /// </remarks>
    /// <seealso cref="IService"/>
    /// <seealso cref="IServiceFactory"/>
    /// <seealso cref="ServiceLocator"/>
    [AttributeUsage(AttributeTargets.Class)]
    [BaseTypeRequired(typeof(IService))]
    public sealed class PreloadServiceAttribute : Attribute { }
}
