using System;
using UnityEngine;

namespace Coimbra
{
    [DisallowMultipleComponent]
    public abstract class ServiceActorBase<T> : Actor, IService
        where T : class, IService
    {
        [SerializeReference]
        [Disable]
        private ServiceLocator _owningLocator;

        static ServiceActorBase()
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentOutOfRangeException($"\"{typeof(ServiceActorBase<>)}\" requires an interface type argument!");
            }

            if (typeof(T) == typeof(IService))
            {
                throw new ArgumentOutOfRangeException($"\"{typeof(ServiceActorBase<>)}\" requires a type different than \"{typeof(IService)}\" itself!");
            }

            if (!typeof(IService).IsAssignableFrom(typeof(T)))
            {
                throw new ArgumentOutOfRangeException($"\"{typeof(ServiceActorBase<>)}\" requires a type that implements \"{typeof(IService)}\"!");
            }
        }

        /// <inheritdoc/>
        public ServiceLocator OwningLocator
        {
            get => _owningLocator;
            set
            {
                if (_owningLocator == value)
                {
                    return;
                }

                ServiceLocator oldValue = _owningLocator;
                _owningLocator = value;
                OnOwningLocatorChanged(oldValue, _owningLocator);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Destroy();
        }

        /// <summary>
        /// Will be called after the <see cref="OwningLocator"/> has been set and only if the value actually changed.
        /// </summary>
        /// <param name="oldValue">The value before.</param>
        /// <param name="newValue">The value after. Is the same as the current <see cref="OwningLocator"/>.</param>
        protected virtual void OnOwningLocatorChanged(ServiceLocator oldValue, ServiceLocator newValue) { }

        /// <inheritdoc/>
        protected override void OnDestroying()
        {
            base.OnDestroying();
            _owningLocator?.Set<T>(null);
            OwningLocator = null;
        }
    }
}
