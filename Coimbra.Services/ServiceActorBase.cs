#nullable enable

using System.ComponentModel;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

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

                _owningLocator = value;

                if (_owningLocator == null)
                {
                    return;
                }

                const string destroyedMessage = nameof(ServiceActorBase<TServiceActor, TService>) + "." + nameof(Destroy) + " was called already but its " + nameof(OwningLocator) + " is being set!";
                Debug.Assert(!IsDestroyed, destroyedMessage, GameObject);

                const string initializedMessage = nameof(ServiceActorBase<TServiceActor, TService>) + "." + nameof(Initialize) + " needs to be called before the " + nameof(OwningLocator) + " is set!";
                Debug.Assert(IsInitialized, initializedMessage, GameObject);
                OnOwningLocatorSet();
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
                _owningLocator.Set<TService>(null);
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
        protected virtual void OnOwningLocatorSet() { }
    }
}
