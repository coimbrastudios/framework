using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        public delegate void ValueChangeEventHandler([CanBeNull] object oldValue, [CanBeNull] object newValue);

        private sealed class Service
        {
            internal bool ResetCreateCallbackOnSet;

            internal object Value;

            internal Func<object> CreateCallback;

            internal ValueChangeEventHandler ValueChangedCallback;

            public Service()
            {
                ValueChangedCallback = HandleValueChanged;
            }

            private void HandleValueChanged(object oldValue, object newValue)
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
        public static readonly ServiceLocator Shared = new ServiceLocator();

        private static readonly Dictionary<Type, Func<object>> DefaultCreateCallbacks = new Dictionary<Type, Func<object>>();
        private readonly Dictionary<Type, Service> _services = new Dictionary<Type, Service>();

        /// <summary>
        /// Sets the default callback for when a service needs to be created.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        public static void SetDefaultCreateCallback<T>([CanBeNull] Func<T> createCallback, bool overrideExisting)
            where T : class
        {
            if (!overrideExisting && DefaultCreateCallbacks.ContainsKey(typeof(T)))
            {
                return;
            }

            if (createCallback != null)
            {
                DefaultCreateCallbacks[typeof(T)] = createCallback;
            }
            else
            {
                DefaultCreateCallbacks.Remove(typeof(T));
            }
        }

        /// <summary>
        /// Adds a listener for when a service instance changes.
        /// </summary>
        /// <param name="callback">The callback to be invoked.</param>
        /// <typeparam name="T">The service type.</typeparam>
        public void AddValueChangedListener<T>([NotNull] ValueChangeEventHandler callback)
            where T : class
        {
            Initialize(typeof(T), out Service service);

            service.ValueChangedCallback += callback;
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
            Initialize(typeof(T), out Service service);

            if (service.CreateCallback == null || service.Value != null)
            {
                return (T)service.Value;
            }

            service.Value = service.CreateCallback.Invoke();

            if (service.ResetCreateCallbackOnSet)
            {
                service.CreateCallback = null;
            }

            T value = (T)service.Value;
            service.ValueChangedCallback(null, value);

            return value;
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
            return _services.TryGetValue(type, out Service service) && service is { CreateCallback: { } };
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
        public void RemoveValueChangedListener<T>([NotNull] ValueChangeEventHandler callback)
            where T : class
        {
            if (_services.TryGetValue(typeof(T), out Service service))
            {
                service.ValueChangedCallback -= callback;
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
            Initialize(typeof(T), out Service service);

            T oldValue = service.Value as T;
            service.Value = value;

            if (service.ResetCreateCallbackOnSet)
            {
                service.CreateCallback = null;
            }

            service.ValueChangedCallback(oldValue, value);
        }

        /// <summary>
        /// Sets the callback for when a service needs to be created.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        public void SetCreateCallback<T>([CanBeNull] Func<T> createCallback, bool resetOnSet)
            where T : class
        {
            Initialize(typeof(T), out Service service);

            service.CreateCallback = createCallback;
            service.ResetCreateCallbackOnSet = resetOnSet;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Initialize(Type type, out Service service)
        {
            if (_services.TryGetValue(type, out service))
            {
                return;
            }

            service = new Service();
            _services[type] = service;
            DefaultCreateCallbacks.TryGetValue(type, out service.CreateCallback);
        }
    }
}
