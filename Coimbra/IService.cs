using System;
using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Base interface for any service to be used with <see cref="ServiceLocator"/>.
    /// </summary>
    [RequireImplementors]
    public interface IService : IDisposable { }
}
