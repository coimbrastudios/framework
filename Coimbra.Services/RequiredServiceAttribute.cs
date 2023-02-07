using JetBrains.Annotations;
using System;
using UnityEngine.Scripting;

namespace Coimbra.Services
{
    /// <summary>
    /// Adding this attribute to an <see cref="IService"/> definition interface lets the code assume that it is never null.
    /// </summary>
    /// <remarks>
    /// This attribute is mostly to allow the <see cref="ServiceLocator.GetChecked{T}"/> API for the given <see cref="IService"/>.
    /// </remarks>
    /// <seealso cref="IService"/>
    /// <seealso cref="IServiceFactory"/>
    /// <seealso cref="ServiceLocator"/>
    [AttributeUsage(AttributeTargets.Interface)]
    [BaseTypeRequired(typeof(IService))]
    [Preserve]
    public sealed class RequiredServiceAttribute : Attribute { }
}
