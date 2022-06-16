using JetBrains.Annotations;
using System;
using UnityEngine.Scripting;

namespace Coimbra.Services
{
    /// <summary>
    /// Add this attribute to an <see cref="IService"/> to enable its value to change at runtime.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    [BaseTypeRequired(typeof(IService))]
    [Preserve]
    public sealed class DynamicServiceAttribute : Attribute { }
}
