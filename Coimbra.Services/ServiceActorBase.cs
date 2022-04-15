using System;
using UnityEngine;

namespace Coimbra.Services
{
    /// <summary>
    /// Base class to easily create a <see cref="IService"/> that is also an <see cref="Actor"/>.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class ServiceActorBase<T> : Actor, IService
        where T : class, IService
    {
        static ServiceActorBase()
        {
            typeof(T).AssertInterfaceImplementsNotEqual<IService>();
        }

        protected ServiceActorBase()
        {
            Type type = GetType();

            if (!type.IsAbstract)
            {
                type.AssertNonInterfaceImplements<IService>();
            }
        }

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
            OwningLocator?.Set<T>(null);
            (this as IService).SetOwningLocator(null);
        }
    }
}
