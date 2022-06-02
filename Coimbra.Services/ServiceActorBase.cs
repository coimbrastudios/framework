#nullable enable

using System.ComponentModel;
using System.Diagnostics;
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
        [SerializeReference]
        [Disable]
        private ServiceLocator? _owningLocator;

        /// <inheritdoc/>
        public ServiceLocator? OwningLocator
        {
            [DebuggerStepThrough]
            get => _owningLocator;
            [EditorBrowsable(EditorBrowsableState.Never)]
            set
            {
                if (_owningLocator == value)
                {
                    return;
                }

                Initialize();

                ServiceLocator? previous = _owningLocator;
                _owningLocator = value;
                OnOwningLocatorChanged(previous, value);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Destroy();
        }

        /// <inheritdoc/>
        protected sealed override void OnDestroyed()
        {
            base.OnDestroyed();

            OnDispose();

            if (_owningLocator == null)
            {
                return;
            }

            if (_owningLocator.IsCreated(out TService? value) && value == this as TService)
            {
                _owningLocator.Set<TService>(null, false);
            }
            else
            {
                ServiceLocator? previous = _owningLocator;
                _owningLocator = null;
                OnOwningLocatorChanged(previous, null);
            }
        }

        /// <summary>
        /// Will be called during <see cref="OnDestroyed"/> before cleaning the <see cref="OwningLocator"/>.
        /// </summary>
        protected virtual void OnDispose() { }

        /// <summary>
        /// Will be called after the <see cref="OwningLocator"/> has been set and only if the value actually changed.
        /// <para></para>
        /// It is guaranteed to run after <see cref="Actor.OnInitialize"/> has been called and should be used for any initialization logic related to other services.
        /// </summary>
        /// <param name="previous">The value before.</param>
        /// <param name="current">The value after. Is the same as the current <see cref="OwningLocator"/>.</param>
        protected virtual void OnOwningLocatorChanged(ServiceLocator? previous, ServiceLocator? current) { }
    }
}
