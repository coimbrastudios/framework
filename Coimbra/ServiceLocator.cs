using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;
using Debug = UnityEngine.Debug;

namespace Coimbra
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
        public delegate void ServiceChangeHandler([CanBeNull] IService previous, [CanBeNull] IService current);

        private sealed class Service
        {
            internal bool ResetCreateCallbackOnSet;

            internal IService Value;

            internal Func<IService> CreateCallback;

            internal ServiceChangeHandler ValueChangedCallback;

            internal Service()
            {
                ValueChangedCallback = HandleValueChanged;
            }

            internal void HandleValueChanged(IService previous, IService current)
            {
                {
                    if (current is MonoBehaviour monoBehaviour && monoBehaviour.TryGetValid(out monoBehaviour))
                    {
                        monoBehaviour.gameObject.AsActor().OnDestroyed += HandleDestroyed;
                    }
                }

                {
                    if (previous is MonoBehaviour monoBehaviour && monoBehaviour.TryGetValid(out monoBehaviour))
                    {
                        monoBehaviour.gameObject.AsActor().OnDestroyed -= HandleDestroyed;
                    }
                }
            }

            private void HandleDestroyed(Actor sender, Actor.DestroyReason destroyReason)
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
        public event Action<ServiceLocator> OnDispose;

        /// <summary>
        /// Default shared service locator. Only use this for services that should be registered within the global scope of the application.
        /// </summary>
        [NotNull]
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
        public string Id { get; private set; }

        /// <summary>
        /// If true and a service is not found, it will try to find the service in the <see cref="Shared"/> instance.
        /// </summary>
        [field: SerializeField]
        public bool AllowFallbackToShared { get; private set; }

        /// <summary>
        /// Adds a listener for when a service instance changes.
        /// </summary>
        /// <param name="callback">The callback to be invoked.</param>
        /// <typeparam name="T">The service type.</typeparam>
        public void AddValueChangedListener<T>([NotNull] ServiceChangeHandler callback)
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
                service.Value.GetValid()?.Dispose();

                if (service.Value.IsValid())
                {
                    service.Value.OwningLocator = null;
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
        [CanBeNull]
        public T Get<T>()
            where T : class, IService
        {
            Initialize(typeof(T), out Service service);

            if (service.Value != null)
            {
                return (T)service.Value;
            }

            T value = null;

            if (service.CreateCallback != null)
            {
                service.Value = service.CreateCallback.Invoke().GetValid();
                value = service.Value as T;

                if (service.ResetCreateCallbackOnSet)
                {
                    service.CreateCallback = null;
                }
            }

            if (value != null)
            {
                value.OwningLocator = this;
            }
            else if (AllowFallbackToShared)
            {
                value = Shared.Get<T>();
            }
            else
            {
                return null;
            }

            if (value == null)
            {
                return null;
            }

            service.ValueChangedCallback(null, value);

            return value;
        }

        /// <summary>
        /// Gets the callback for when a service needs to be created.
        /// </summary>
        /// <param name="willResetOnSet">If the create callback will reset when the service is set.</param>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>The create callback, if set.</returns>
        [CanBeNull]
        public Func<IService> GetCreateCallback<T>(out bool willResetOnSet)
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
        [CanBeNull]
        public Func<IService> GetCreateCallback(Type type, out bool willResetOnSet)
        {
            type.AssertInterfaceImplementsNotEqual<IService>();

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
        public bool HasCreateCallback([NotNull] Type type)
        {
            type.AssertInterfaceImplementsNotEqual<IService>();

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
        public bool IsCreated([NotNull] Type type)
        {
            type.AssertInterfaceImplementsNotEqual<IService>();

            return _services.TryGetValue(type, out Service service) && service is { Value: { } };
        }

        /// <summary>
        /// Removes a listener for when a service instance changes.
        /// </summary>
        /// <param name="callback">The callback to be removed.</param>
        /// <typeparam name="T">The service type.</typeparam>
        public void RemoveValueChangedListener<T>([NotNull] ServiceChangeHandler callback)
            where T : class, IService
        {
            typeof(T).AssertInterfaceImplementsNotEqual<IService>();

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
        public void Set<T>([CanBeNull] T value, bool disposePrevious = false)
            where T : class, IService
        {
            Initialize(typeof(T), out Service service);

            if (value is { OwningLocator: { } })
            {
                if (value.OwningLocator != this)
                {
                    Debug.LogError($"The same service \"{value}\" can't belong to more than one {nameof(ServiceLocator)} at same time!");
                }

                return;
            }

            T oldValue = (service.Value as T).GetValid();
            service.Value = value.GetValid();

            if (service.ResetCreateCallbackOnSet)
            {
                service.CreateCallback = null;
            }

            if (oldValue != null && oldValue.OwningLocator == this)
            {
                if (disposePrevious)
                {
                    oldValue.Dispose();
                }

                oldValue.OwningLocator = null;
            }

            if (service.Value != null)
            {
                service.Value.OwningLocator = this;
            }

            service.ValueChangedCallback(oldValue, service.Value);
        }

        /// <summary>
        /// Sets the callback for when a service needs to be created.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        public void SetCreateCallback<T>([CanBeNull] Func<T> createCallback, bool resetOnSet)
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
        public bool TryGet<T>([CanBeNull] out T value)
            where T : class, IService
        {
            value = Get<T>();

            return value != null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Initialize(Type type, out Service service, [CallerMemberName] string memberName = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = 0)
        {
            type.AssertInterfaceImplementsNotEqual<IService>(memberName);

            if (_services.TryGetValue(type, out service))
            {
                return;
            }

            service = new Service();
            _services[type] = service;
        }
    }
}
