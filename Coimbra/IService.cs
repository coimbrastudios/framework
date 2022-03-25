using System;
using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Base interface for any service to be used with <see cref="ServiceLocator"/>.
    /// </summary>
    [RequireImplementors]
    public interface IService : IDisposable
    {
        /// <summary>
        /// The <see cref="ServiceLocator"/> that owns this service.
        /// <para></para><see cref="set_OwningLocator"/> is for protected and internal use only.
        /// </summary>
        ServiceLocator OwningLocator { get; set; }
    }
}
