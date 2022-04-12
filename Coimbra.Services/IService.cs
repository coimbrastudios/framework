using System;
using UnityEngine.Scripting;

namespace Coimbra.Services
{
    /// <summary>
    /// Base interface for any service to be used with <see cref="ServiceLocator"/>.
    /// </summary>
    [RequireImplementors]
    public interface IService : IDisposable
    {
        /// <summary>
        /// The <see cref="ServiceLocator"/> that owns this service.
        /// <para><see cref="set_OwningLocator"/> is for protected and internal use only.</para>
        /// </summary>
        ServiceLocator OwningLocator { get; set; }
    }
}
