#nullable enable

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;
using Debug = UnityEngine.Debug;

namespace Coimbra.Services
{
    /// <summary>
    /// A non-thread-safe service locator.
    /// </summary>
    [Preserve]
    [Serializable]
    public sealed class ServiceLocator : IDisposable
    {
        /// <summary>
        /// Delegate for listening when a service is set.
        /// </summary>
        public delegate void SetHandler(ServiceLocator serviceLocator);

        internal sealed class Service
        {
            internal IService? Value;

            internal IServiceFactory? Factory;

            internal SetHandler? SetHandler;
        }

        /// <summary>
        /// Called when this <see cref="ServiceLocator"/> is about to be disposed.
        /// </summary>
        public event Action<ServiceLocator>? OnDispose;

        /// <summary>
        /// Default shared service locator. Only use this for services that should be registered within the global scope of the application.
        /// </summary>
        public static readonly ServiceLocator Shared;

        internal static readonly Dictionary<string, WeakReference<ServiceLocator>> ServiceLocators;

        private readonly Dictionary<Type, Service> _services = new Dictionary<Type, Service>();

        static ServiceLocator()
        {
            ServiceLocators = new Dictionary<string, WeakReference<ServiceLocator>>();
            Shared = new ServiceLocator($"{typeof(ServiceLocator).FullName}.{nameof(Shared)}", false);
        }

        /// <param name="id">Identifier that can be used to debugging same <see cref="IService"/> across different <see cref="ServiceLocator"/>.</param>
        /// <param name="allowFallbackToShared">If true and a service is not found, it will try to find the service in the <see cref="Shared"/> instance.</param>
        public ServiceLocator(string id, bool allowFallbackToShared = true)
        {
            if (ServiceLocators.ContainsKey(id))
            {
                throw new ArgumentException($"{nameof(ServiceLocator)} with the given id {id} already exists!", nameof(id));
            }

            AllowFallbackToShared = allowFallbackToShared;
            Id = id;
            _services.Clear();
            ServiceLocators.Add(id, new WeakReference<ServiceLocator>(this));
        }

        ~ServiceLocator()
        {
            Clear();
            ServiceLocators.Remove(Id);
        }

        /// <summary>
        /// Identifier that can be used to debugging same <see cref="IService"/> across different <see cref="ServiceLocator"/>.
        /// </summary>
        [field: SerializeField]
        [field: Disable]
        public string Id { get; private set; }

        /// <summary>
        /// If true and a service is not found, it will try to find the service in the <see cref="Shared"/> instance.
        /// </summary>
        [field: SerializeField]
        [field: Disable]
        public bool AllowFallbackToShared { get; private set; }

        internal IReadOnlyDictionary<Type, Service> Services => _services;

        /// <summary>
        /// Adds a listener for when a service instance changes.
        /// </summary>
        /// <param name="callback">The callback to be invoked.</param>
        /// <typeparam name="T">The service type.</typeparam>
        public void AddSetListener<T>(SetHandler callback)
            where T : class, IService
        {
            Initialize(typeof(T), out Service service);

            service.SetHandler += callback;
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            if (this == Shared)
            {
                Debug.LogWarning($"{nameof(ServiceLocator)}.{nameof(Shared)}.{nameof(Dispose)} is no-op.");

                return;
            }

            OnDispose?.Invoke(this);
            Clear();
        }

        /// <summary>
        /// Gets a service instance.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>The service instance.</returns>
        public T? Get<T>()
            where T : class, IService
        {
            Initialize(typeof(T), out Service service);

            if (service.Value != null)
            {
                return service.Value as T;
            }

            if (service.Factory == null)
            {
                return AllowFallbackToShared ? Shared.Get<T>() : null;
            }

            IService? value = service.Factory.Create(this).GetValid();

            switch (value)
            {
                case null:
                {
                    return AllowFallbackToShared ? Shared.Get<T>() : null;
                }

                case T result:
                {
                    Set(result);

                    return result;
                }

                default:
                {
                    Debug.LogWarning($"Create callback for {typeof(T)} returning a value of type {value.GetType()}! Disposing it...");
                    value.Dispose();

                    return AllowFallbackToShared ? Shared.Get<T>() : null;
                }
            }
        }

        /// <summary>
        /// Gets the factory for a service type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>The factory, if set.</returns>
        public IServiceFactory? GetFactory<T>()
            where T : class, IService
        {
            return GetFactory(typeof(T));
        }

        /// <summary>
        /// Gets the factory for a service type.
        /// </summary>
        /// <param name="type">The service type.</param>
        /// <returns>The factory, if set.</returns>
        public IServiceFactory? GetFactory(Type type)
        {
            return _services.TryGetValue(type, out Service service) ? service.Factory : null;
        }

        /// <summary>
        /// Gets if the factory is set for a service type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>False if the service has no factory set.</returns>
        [Pure]
        public bool HasFactory<T>()
            where T : class, IService
        {
            return HasFactory(typeof(T));
        }

        /// <summary>
        /// Gets if the factory is set for a service type.
        /// </summary>
        /// <param name="type">The service type.</param>
        /// <returns>False if the service has no factory set.</returns>
        [Pure]
        public bool HasFactory(Type type)
        {
            return _services.TryGetValue(type, out Service service) && service is { Factory: { } };
        }

        /// <summary>
        /// Gets if the service is created by its type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>False if the service wasn't created.</returns>
        [Pure]
        public bool IsCreated<T>()
            where T : class, IService
        {
            return IsCreated(typeof(T));
        }

        /// <summary>
        /// Gets if the service is created by its type.
        /// </summary>
        /// <param name="type">The service type.</param>
        /// <returns>False if the service wasn't created.</returns>
        [Pure]
        public bool IsCreated(Type type)
        {
            return _services.TryGetValue(type, out Service service) && service.Value != null;
        }

        /// <summary>
        /// Gets if the service is created by its type.
        /// </summary>
        /// <param name="value">The service value, if created.</param>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>False if the service wasn't created.</returns>
        [Pure]
        public bool IsCreated<T>(out T? value)
            where T : class, IService
        {
            if (IsCreated(typeof(T), out IService? service))
            {
                value = service as T;

                return true;
            }

            value = null;

            return false;
        }

        /// <summary>
        /// Gets if the service is created by its type.
        /// </summary>
        /// <param name="type">The service type.</param>
        /// <param name="value">The service value, if created.</param>
        /// <returns>False if the service wasn't created.</returns>
        [Pure]
        public bool IsCreated(Type type, out IService? value)
        {
            if (_services.TryGetValue(type, out Service service) && service.Value != null)
            {
                value = service.Value;

                return true;
            }

            value = null;

            return false;
        }

        /// <summary>
        /// Removes a listener for when a service instance changes.
        /// </summary>
        /// <param name="callback">The callback to be removed.</param>
        /// <typeparam name="T">The service type.</typeparam>
        public void RemoveSetListener<T>(SetHandler callback)
            where T : class, IService
        {
            if (_services.TryGetValue(typeof(T), out Service service))
            {
                service.SetHandler -= callback;
            }
        }

        /// <summary>
        /// Sets a service instance.
        /// </summary>
        /// <param name="value">The service instance.</param>
        /// <typeparam name="T">The service type.</typeparam>
        public void Set<T>(T? value)
            where T : class, IService
        {
            Initialize(typeof(T), out Service service);

            if (service.Value.TryGetValid(out service.Value))
            {
                if (value.TryGetValid(out value))
                {
                    Debug.LogError($"Service of type {typeof(T)} is already set at {nameof(ServiceLocator)} with id \"{Id}\" to \"{value}\"!");

                    return;
                }

                service.Value!.Dispose();

                service.Value = null;
                service.SetHandler?.Invoke(this);
            }
            else if (value.TryGetValid(out value!))
            {
                if (value.OwningLocator != null)
                {
                    Debug.LogError($"The same service \"{value}\" can't belong to more than one {nameof(ServiceLocator)} at same time!");

                    return;
                }

                service.Value = value;
                value.OwningLocator = this;
                service.SetHandler?.Invoke(this);
            }
        }

        /// <summary>
        /// Sets the factory for when a service needs to be created.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        public void SetFactory<T>(IServiceFactory? factory)
            where T : class, IService
        {
            Initialize(typeof(T), out Service service);

            service.Factory = factory;
        }

        /// <summary>
        /// Tries to get a service instance.
        /// </summary>
        /// <param name="value">The service instance.</param>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>False if the service does not exists.</returns>
        public bool TryGet<T>(out T? value)
            where T : class, IService
        {
            value = Get<T>();

            return value != null;
        }

        internal void Clear()
        {
            foreach (Service service in _services.Values)
            {
                service.Factory = null;
                service.SetHandler = null;

                if (service.Value.TryGetValid(out service.Value!))
                {
                    service.Value.Dispose();

                    if (service.Value.TryGetValid(out service.Value!))
                    {
                        service.Value.OwningLocator = null;
                    }
                }

                service.Value = null;
            }

            _services.Clear();
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
        }
    }
}
