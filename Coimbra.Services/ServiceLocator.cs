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
        /// Delegate for listener for service instance changes.
        /// </summary>
        public delegate void ServiceChangeHandler(IService? previous, IService? current);

        private sealed class Service
        {
            internal bool ResetCreateCallbackOnSet;

            internal IService? Value;

            internal Func<IService>? CreateCallback;

            internal ServiceChangeHandler? ValueChangedCallback;

            internal Service()
            {
                ValueChangedCallback = HandleValueChanged;
            }

            internal void HandleValueChanged(IService? previous, IService? current)
            {
                {
                    if (current is MonoBehaviour monoBehaviour && monoBehaviour.TryGetValid(out monoBehaviour))
                    {
                        monoBehaviour.gameObject.AsActor().OnDestroying += HandleDestroying;
                    }
                }

                {
                    if (previous is MonoBehaviour monoBehaviour && monoBehaviour.TryGetValid(out monoBehaviour))
                    {
                        monoBehaviour.gameObject.AsActor().OnDestroying -= HandleDestroying;
                    }
                }
            }

            private void HandleDestroying(Actor sender, Actor.DestroyReason destroyReason)
            {
                if (Value is MonoBehaviour monoBehaviour && monoBehaviour.gameObject == sender.CachedGameObject)
                {
                    Value = null;
                }
            }
        }

        /// <summary>
        /// Called when this <see cref="ServiceLocator"/> is about to be disposed.
        /// </summary>
        public event Action<ServiceLocator>? OnDispose;

        /// <summary>
        /// Default shared service locator. Only use this for services that should be registered within the global scope of the application.
        /// </summary>
        public static readonly ServiceLocator Shared = new ServiceLocator($"{typeof(ServiceLocator).FullName}.{nameof(Shared)}", false);

        private readonly Dictionary<Type, Service> _services = new Dictionary<Type, Service>();

        /// <param name="id">Identifier that can be used to debugging same <see cref="IService"/> across different <see cref="ServiceLocator"/>.</param>
        /// <param name="allowFallbackToShared">If true and a service is not found, it will try to find the service in the <see cref="Shared"/> instance.</param>
        public ServiceLocator(string id, bool allowFallbackToShared = true)
        {
            AllowFallbackToShared = allowFallbackToShared;
            Id = id;
            _services.Clear();
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

        /// <summary>
        /// Adds a listener for when a service instance changes.
        /// </summary>
        /// <param name="callback">The callback to be invoked.</param>
        /// <typeparam name="T">The service type.</typeparam>
        public void AddValueChangedListener<T>(ServiceChangeHandler callback)
            where T : class, IService
        {
            Initialize(typeof(T), out Service service);

            service.ValueChangedCallback += callback;
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            OnDispose?.Invoke(this);

            foreach (Service service in _services.Values)
            {
                service.ValueChangedCallback = service.HandleValueChanged;

                if (service.Value.TryGetValid(out service.Value!))
                {
                    service.Value.Dispose();

                    if (service.Value.TryGetValid(out service.Value!))
                    {
                        service.Value.OwningLocator = null;
                    }
                }

                service.Value = null;
                service.CreateCallback = null;
            }

            _services.Clear();
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

            if (service.CreateCallback == null)
            {
                return AllowFallbackToShared ? Shared.Get<T>() : null;
            }

            IService? value = service.CreateCallback.Invoke();

            switch (value)
            {
                case null:
                {
                    return AllowFallbackToShared ? Shared.Get<T>() : null;
                }

                case T result:
                {
                    Set(result, false);

                    return result;
                }
            }

            Debug.LogWarning($"Create callback for {typeof(T)} returning a value of type {value.GetType()}! Disposing it...");
            value.Dispose();

            return AllowFallbackToShared ? Shared.Get<T>() : null;
        }

        /// <summary>
        /// Gets the callback for when a service needs to be created.
        /// </summary>
        /// <param name="willResetOnSet">If the create callback will reset when the service is set.</param>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>The create callback, if set.</returns>
        public Func<IService>? GetCreateCallback<T>(out bool willResetOnSet)
            where T : class, IService
        {
            return GetCreateCallback(typeof(T), out willResetOnSet);
        }

        /// <summary>
        /// Gets the callback for when a service needs to be created.
        /// </summary>
        /// <param name="type">The service type.</param>
        /// <param name="willResetOnSet">If the create callback will reset when the service is set.</param>
        /// <returns>The create callback, if set.</returns>
        public Func<IService>? GetCreateCallback(Type type, out bool willResetOnSet)
        {
            if (_services.TryGetValue(type, out Service service))
            {
                willResetOnSet = service.ResetCreateCallbackOnSet;

                return service.CreateCallback;
            }

            willResetOnSet = false;

            return null;
        }

        /// <summary>
        /// Gets if the create callback is currently set for a service type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>False if the service has no create callback set or the service type is not found.</returns>
        [Pure]
        public bool HasCreateCallback<T>()
            where T : class, IService
        {
            return HasCreateCallback(typeof(T));
        }

        /// <summary>
        /// Gets if the create callback is currently set for a service type.
        /// </summary>
        /// <param name="type">The service type.</param>
        /// <returns>False if the service has no create callback set or the service type is not found.</returns>
        [Pure]
        public bool HasCreateCallback(Type type)
        {
            return _services.TryGetValue(type, out Service service) && service is { CreateCallback: { } };
        }

        /// <summary>
        /// Gets if the service is created by its type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>False if the service wasn't created or the service type is not found.</returns>
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
        /// <returns>False if the service wasn't created or the service type is not found.</returns>
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
        /// <returns>False if the service wasn't created or the service type is not found.</returns>
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
        /// <returns>False if the service wasn't created or the service type is not found.</returns>
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
        public void RemoveValueChangedListener<T>(ServiceChangeHandler callback)
            where T : class, IService
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
        /// <param name="disposePrevious">If true, the last set instance will have their <see cref="IDisposable.Dispose"/> method called.</param>
        /// <typeparam name="T">The service type.</typeparam>
        public void Set<T>(T? value, bool disposePrevious)
            where T : class, IService
        {
            Initialize(typeof(T), out Service service);

            if (service.Value == value)
            {
                return;
            }

            T? oldValue = (service.Value as T).GetValid();

            if (value.TryGetValid(out value!))
            {
                if (value.OwningLocator != null)
                {
                    Debug.LogError($"The same service \"{value}\" can't belong to more than one {nameof(ServiceLocator)} at same time!");

                    return;
                }

                service.Value = value;
                value.OwningLocator = this;
            }
            else
            {
                value = AllowFallbackToShared ? Shared.Get<T>() : null;
                service.Value = null;
            }

            if (service.ResetCreateCallbackOnSet)
            {
                service.CreateCallback = null;
            }

            if (oldValue != null)
            {
                if (disposePrevious)
                {
                    oldValue.Dispose();
                }

                oldValue.OwningLocator = null;
            }
            else if (AllowFallbackToShared)
            {
                oldValue = Shared.Get<T>();
            }

            service.ValueChangedCallback!(oldValue, value);
        }

        /// <summary>
        /// Sets the callback for when a service needs to be created.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        public void SetCreateCallback<T>(Func<T>? createCallback, bool resetOnSet)
            where T : class, IService
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
        public bool TryGet<T>(out T? value)
            where T : class, IService
        {
            value = Get<T>();

            return value != null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void HandleSubsystemRegistration()
        {
            Application.quitting -= HandleApplicationQuitting;
            Application.quitting += HandleApplicationQuitting;
        }

        private static void HandleApplicationQuitting()
        {
            Shared.Dispose();
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
