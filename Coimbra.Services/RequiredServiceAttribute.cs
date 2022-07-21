using JetBrains.Annotations;
using System;
using UnityEngine.Scripting;

namespace Coimbra.Services
{
    /// <summary>
    /// Adding this attribute to an <see cref="IService"/> lets the code assume that it is never null.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    [BaseTypeRequired(typeof(IService))]
    [Preserve]
    public sealed class RequiredServiceAttribute : Attribute { }
}
