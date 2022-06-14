// ReSharper disable RequiredBaseTypesIsNotInherited

#nullable enable

using System;
using UnityEngine.Scripting;

namespace Coimbra.Services
{
    /// <summary>
    /// Base interface for any service to be used with <see cref="ServiceLocator"/>.
    /// </summary>
    [AbstractService]
    [RequireImplementors]
    public interface IService : IDisposable { }
}
