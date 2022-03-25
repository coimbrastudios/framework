using System;
using UnityEngine;

namespace Coimbra
{
    public abstract class MonoBehaviourServiceBase<T> : MonoBehaviour, IService
        where T : class, IService
    {
        [SerializeReference]
        [Disable]
        private ServiceLocator _owningLocator;

        static MonoBehaviourServiceBase()
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentOutOfRangeException($"\"{typeof(MonoBehaviourServiceBase<>)}\" requires an interface type argument!");
            }

            if (typeof(T) == typeof(IService))
            {
                throw new ArgumentOutOfRangeException($"\"{typeof(MonoBehaviourServiceBase<>)}\" requires a type different than \"{typeof(IService)}\" itself!");
            }

            if (!typeof(IService).IsAssignableFrom(typeof(T)))
            {
                throw new ArgumentOutOfRangeException($"\"{typeof(MonoBehaviourServiceBase<>)}\" requires a type that implements \"{typeof(IService)}\"!");
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
