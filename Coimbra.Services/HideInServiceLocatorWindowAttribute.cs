using JetBrains.Annotations;
using System;

namespace Coimbra.Services
{
    /// <summary>
    /// Apply this to any <see cref="IService"/> that should not appear in the <see cref="ServiceLocator"/> window.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    [BaseTypeRequired(typeof(IService))]
    public class HideInServiceLocatorWindowAttribute : Attribute { }
}
