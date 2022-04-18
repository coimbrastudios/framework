using UnityEngine;

namespace Coimbra.Services
{
    /// <summary>
    /// Base class to easily create a <see cref="IService"/> that is also an <see cref="Actor"/>.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class ServiceActorBase<TServiceActor, TService> : Actor, IService
        where TServiceActor : ServiceActorBase<TServiceActor, TService>, TService
        where TService : class, IService
    {
        /// <inheritdoc/>
        [field: SerializeReference]
        [field: Disable]
        public ServiceLocator OwningLocator { get; set; }

        /// <inheritdoc/>
        public void Dispose()
        {
            Destroy();
        }

        /// <inheritdoc/>
        protected override void OnDestroying()
        {
            base.OnDestroying();

            if (OwningLocator == null)
            {
                return;
            }

            if (OwningLocator.IsCreated(out TService value) && value == this as TService)
            {
                OwningLocator.Set<TService>(null, false);
            }

            OwningLocator = null;
        }
    }
}
