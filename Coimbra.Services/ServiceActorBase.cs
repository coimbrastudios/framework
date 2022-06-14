#nullable enable

using UnityEngine;

namespace Coimbra.Services
{
    /// <summary>
    /// Base class to easily create a <see cref="IService"/> that is also an <see cref="Actor"/>.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class ServiceActorBase : Actor, IService
    {
        /// <inheritdoc/>
        public void Dispose()
        {
            Destroy();
        }
    }
}
