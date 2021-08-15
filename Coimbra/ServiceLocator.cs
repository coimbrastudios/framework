using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// A non-thread-safe service locator.
    /// </summary>
    [Preserve]
    public sealed class ServiceLocator
    {
        public delegate void ValueChangeEventHandler<in T>([CanBeNull] T oldValue, [CanBeNull] T newValue)
            where T : class;

        private static class ServiceLocatorT<T>
            where T : class
        {
            internal static readonly Dictionary<ServiceLocator, Func<T>> CreateCallbacks = new Dictionary<ServiceLocator, Func<T>>(1);
            internal static readonly Dictionary<ServiceLocator, ValueChangeEventHandler<T>> ValueChangedCallbacks = new Dictionary<ServiceLocator, ValueChangeEventHandler<T>>(1);
        }

        private sealed class Service
        {
            internal bool HasCreateCallback;

            internal bool AutoResetCreateCallback;

            internal object Value;

            internal void HandleValueChanged(object oldValue, object newValue)
            {
                {
                    if (newValue is GameObject gameObject)
                    {
                        gameObject.AddDestroyEventListener(HandleGameObjectDestroy);
                    }
                }

                {
                    if (oldValue is GameObject gameObject && gameObject != null)
                    {
                        gameObject.RemoveDestroyEventListener(HandleGameObjectDestroy);
                    }
                }
            }

            private void HandleGameObjectDestroy(GameObjectEventListener sender, DestroyEventType destroyEventType)
            {
                if (Value as GameObject == sender.gameObject)
                {
                    Value = null;
                }
            }
        }

        /// <summary>
        /// Default shared service locator.
        /// </summary>
        [NotNull]
        public static readonly ServiceLocator Global = new ServiceLocator();

        private readonly Dictionary<Type, Service> _services = new Dictionary<Type, Service>();

        /// <summary>
        /// Adds a listener for when a service instance changes.
        /// </summary>
        /// <param name="callback">The callback to be invoked.</param>
        /// <typeparam name="T">The service type.</typeparam>
        public void AddValueChangedListener<T>([NotNull] ValueChangeEventHandler<T> callback)
            where T : class
        {
            if (!_services.TryGetValue(typeof(T), out _))
            {
                Initialize<T>(out _);
            }

            ServiceLocatorT<T>.ValueChangedCallbacks[this] += callback;
        }

        /// <summary>
        /// Gets a service instance.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>The service instance.</returns>
        [CanBeNull]
        [Pure]
        public T Get<T>()
            where T : class
        {
            if (!_services.TryGetValue(typeof(T), out Service service))
            {
                Initialize<T>(out service);

                return null;
            }

            if (!service.HasCreateCallback || service.Value != null)
            {
                return (T)service.Value;
            }

            service.Value = ServiceLocatorT<T>.CreateCallbacks[this].Invoke();

            if (service.AutoResetCreateCallback)
            {
                service.HasCreateCallback = false;
                ServiceLocatorT<T>.CreateCallbacks[this] = null;
            }

            ServiceLocatorT<T>.ValueChangedCallbacks[this](null, (T)service.Value);

            return (T)service.Value;
        }

        /// <summary>
        /// Gets if the create callback is currently set for a service type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>False if the service has no create callback set or the service type is not found.</returns>
        public bool HasCreateCallback<T>()
            where T : class
        {
            return HasCreateCallback(typeof(T));
        }

        /// <summary>
        /// Gets if the create callback is currently set for a service type.
        /// </summary>
        /// <param name="type">The service type.</param>
        /// <returns>False if the service has no create callback set or the service type is not found.</returns>
        public bool HasCreateCallback([NotNull] Type type)
        {
            return _services.TryGetValue(type, out Service service) && service is { HasCreateCallback: true };
        }

        /// <summary>
        /// Gets if the service is created by its type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>False if the service wasn't created or the service type is not found.</returns>
        public bool IsCreated<T>()
            where T : class
        {
            return IsCreated(typeof(T));
        }

        /// <summary>
        /// Gets if the service is created by its type.
        /// </summary>
        /// <param name="type">The service type.</param>
        /// <returns>False if the service wasn't created or the service type is not found.</returns>
        public bool IsCreated([NotNull] Type type)
        {
            return _services.TryGetValue(type, out Service service) && service is { Value: { } };
        }

        /// <summary>
        /// Removes a listener for when a service instance changes.
        /// </summary>
        /// <param name="callback">The callback to be removed.</param>
        /// <typeparam name="T">The service type.</typeparam>
        public void RemoveValueChangedListener<T>([NotNull] ValueChangeEventHandler<T> callback)
            where T : class
        {
            if (_services.TryGetValue(typeof(T), out _))
            {
                ServiceLocatorT<T>.ValueChangedCallbacks[this] -= callback;
            }
        }

        /// <summary>
        /// Sets a service instance.
        /// </summary>
        /// <param name="value">The service instance.</param>
        /// <typeparam name="T">The service type.</typeparam>
        public void Set<T>([CanBeNull] T value)
            where T : class
        {
            if (!_services.TryGetValue(typeof(T), out Service service))
            {
                Initialize<T>(out service);
            }

            T oldValue = (T)service.Value;
            service.Value = value;
            ServiceLocatorT<T>.ValueChangedCallbacks[this](oldValue, value);
        }

        /// <summary>
        /// Sets the callback for when a service needs to be created.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        public void SetCreateCallback<T>([CanBeNull] Func<T> createCallback, bool autoReset)
            where T : class
        {
            if (!_services.TryGetValue(typeof(T), out Service service))
            {
                Initialize<T>(out service);
            }

            service.AutoResetCreateCallback = autoReset;
            service.HasCreateCallback = createCallback != null;
            ServiceLocatorT<T>.CreateCallbacks[this] = createCallback;
        }

        /// <summary>
        /// Tries to get a service instance.
        /// </summary>
        /// <param name="value">The service instance.</param>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>False if the service does not exists.</returns>
        [Pure]
        public bool TryGet<T>([CanBeNull] out T value)
            where T : class
        {
            value = Get<T>();

            return value != null;
        }

        private void Initialize<T>(out Service service)
            where T : class
        {
            service = new Service();
            _services[typeof(T)] = service;
            ServiceLocatorT<T>.CreateCallbacks[this] = null;
            ServiceLocatorT<T>.ValueChangedCallbacks[this] = service.HandleValueChanged;
        }
    }
}
