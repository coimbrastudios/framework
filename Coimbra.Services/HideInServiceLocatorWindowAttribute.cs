using JetBrains.Annotations;
using System;

namespace Coimbra.Services
{
    /// <summary>
    /// Apply this to any <see cref="IService"/> definition interface or implementation class that should not appear in the <see cref="ServiceLocator"/> window.
    /// </summary>
    /// <remarks>
    /// By default, all services will appear in the <b>Service Locator</b> window. Use this attribute to hide unwanted services there.
    /// <para></para>
    /// Should be used on <see cref="IService"/> types that are meant to be internal or only for testing.
    /// </remarks>
    /// <seealso cref="IService"/>
    /// <seealso cref="IServiceFactory"/>
    /// <seealso cref="ServiceLocator"/>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    [BaseTypeRequired(typeof(IService))]
    public class HideInServiceLocatorWindowAttribute : Attribute { }
}
