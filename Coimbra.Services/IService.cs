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
    public interface IService : IDisposable
    {
        /// <summary>
        /// The <see cref="ServiceLocator"/> that owns this service. <see cref="set_OwningLocator"/> is for internal use only.
        /// </summary>
        ServiceLocator? OwningLocator { get; set; }
    }
}
