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
        /// Default shared service locator. Only use this for services that should be registered within the global scope of the application.
        /// </summary>
        [NotNull]
        public static readonly ServiceLocator Shared = new ServiceLocator(false);

        private static readonly Dictionary<Type, Func<object>> DefaultCreateCallbacks = new Dictionary<Type, Func<object>>();

        /// <summary>
        /// If true and a service is not found, it will try to find the service in the <see cref="Shared"/> instance.
        /// </summary>
        public readonly bool AllowFallbackToShared;

        /// <summary>
        /// If true, it will throw an <see cref="ArgumentOutOfRangeException"/> on any API used with non-interface type arguments.
        /// </summary>
        public readonly bool AllowInterfacesOnly;

        private readonly Dictionary<Type, Service> _services = new Dictionary<Type, Service>();

        /// <param name="allowFallbackToShared">If true and a service is not found, it will try to find the service in the <see cref="Shared"/> instance.</param>
        /// <param name="allowInterfacesOnly">If true, it will throw an <see cref="ArgumentOutOfRangeException"/> on any API used with non-interface type arguments.</param>
        public ServiceLocator(bool allowFallbackToShared = true, bool allowInterfacesOnly = true)
        {
            AllowFallbackToShared = allowFallbackToShared;
            AllowInterfacesOnly = allowInterfacesOnly;
        }

        /// <summary>
        /// Sets the default callback for when a service needs to be created.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        public static void SetDefaultCreateCallback<T>([CanBeNull] Func<T> createCallback, bool overrideExisting, bool allowInterfacesOnly = true)
            where T : class
        {
            AssertTypeIsInterface(allowInterfacesOnly, typeof(T));

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

            if (service.Value != null)
            {
                return (T)service.Value;
            }

            T value = null;

            if (service.CreateCallback != null)
            {
                service.Value = service.CreateCallback.Invoke();

                if (service.ResetCreateCallbackOnSet)
                {
                    service.CreateCallback = null;
                }

                value = service.Value as T;
            }

            if (AllowFallbackToShared && value == null)
            {
                value = Shared.Get<T>();
            }

            if (value == null)
            {
                return null;
            }

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
            AssertTypeIsInterface(type);

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
            AssertTypeIsInterface(type);

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
            AssertTypeIsInterface(typeof(T));

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
            service.Value = value.GetValid();

            if (service.ResetCreateCallbackOnSet)
            {
                service.CreateCallback = null;
            }

            service.ValueChangedCallback(oldValue.GetValid(), value);
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
        private static void AssertTypeIsInterface(bool assert, Type type, [CallerMemberName] string memberName = null)
        {
            if (assert && !type.IsInterface)
            {
                throw new ArgumentOutOfRangeException($"{nameof(ServiceLocator)}.{memberName} requires an interface as the type argument!");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AssertTypeIsInterface(Type type, [CallerMemberName] string memberName = null)
        {
            AssertTypeIsInterface(AllowInterfacesOnly, type, memberName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Initialize(Type type, out Service service, [CallerMemberName] string memberName = null)
        {
            AssertTypeIsInterface(type, memberName);

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
