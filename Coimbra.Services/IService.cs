// ReSharper disable RequiredBaseTypesIsNotInherited

#nullable enable

using System;
using UnityEngine.Scripting;

namespace Coimbra.Services
{
    /// <summary>
    /// Base interface for any service to be used with <see cref="ServiceLocator"/>.
    /// </summary>
    /// <remarks>
    /// There are a few different attributes that can be used in custom services.
    /// <list type="bullets">
    /// <item>
    /// Definition interfaces attributes:
    /// <list type="bullets">
    /// <item><see cref="AbstractServiceAttribute"/></item>
    /// <item><see cref="DynamicServiceAttribute"/></item>
    /// <item><see cref="RequiredServiceAttribute"/></item>
    /// <item><see cref="HideInServiceLocatorWindowAttribute"/></item>
    /// </list>
    /// </item>
    /// <item>
    /// Implementation class attributes:
    /// <list type="bullets">
    /// <item><see cref="DisableDefaultFactoryAttribute"/></item>
    /// <item><see cref="PreloadServiceAttribute"/></item>
    /// <item><see cref="HideInServiceLocatorWindowAttribute"/></item>
    /// </list>
    /// </item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IServiceFactory"/>
    /// <seealso cref="ServiceLocator"/>
    [AbstractService]
    [RequireImplementors]
    public interface IService : IDisposable { }
}
