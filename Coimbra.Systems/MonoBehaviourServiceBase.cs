using System;
using UnityEngine;

namespace Coimbra.Systems
{
    public abstract class MonoBehaviourServiceBase<T> : MonoBehaviour, IService
        where T : class, IService
    {
        [SerializeReference]
        private ServiceLocator _owningLocator;

        /// <inheritdoc cref="IService.OwningLocator"/>
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

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            OnDispose();

            _owningLocator?.Set<T>(null);

            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Called on beginning of <see cref="Dispose"/> before attempting to destroy the gameObject
        /// </summary>
        protected virtual void OnDispose() { }

        /// <summary>
        /// Will be called after the <see cref="OwningLocator"/> has been set and only if the value actually changed.
        /// </summary>
        /// <param name="oldValue">The value before.</param>
        /// <param name="newValue">The value after. Is the same as the current <see cref="OwningLocator"/>.</param>
        protected virtual void OnOwningLocatorChanged(ServiceLocator oldValue, ServiceLocator newValue) { }
    }
}
