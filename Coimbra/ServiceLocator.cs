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
    [Serializable]
    public sealed class ServiceLocator : IDisposable
    {
        private sealed class Service
        {
            internal bool ResetCreateCallbackOnSet;

            internal IService Value;

            internal Func<IService> CreateCallback;

            internal ValueChangeEventHandler ValueChangedCallback;

            internal Service()
            {
                ValueChangedCallback = HandleValueChanged;
            }

            internal void HandleValueChanged(IService oldValue, IService newValue)
            {
                {
                    if (newValue is MonoBehaviour monoBehaviour)
                    {
                        monoBehaviour.GetValid()?.gameObject.AddDestroyEventListener(HandleGameObjectDestroy);
                    }
                }

                {
                    if (oldValue is MonoBehaviour monoBehaviour)
                    {
                        monoBehaviour.GetValid()?.gameObject.RemoveDestroyEventListener(HandleGameObjectDestroy);
                    }
                }
            }

            private void HandleGameObjectDestroy(GameObject sender, DestroyEventType destroyEventType)
            {
                if (Value is MonoBehaviour monoBehaviour && monoBehaviour.gameObject == sender)
                {
                    Value = null;
                }
            }
        }

        public delegate void ValueChangeEventHandler([CanBeNull] IService oldValue, [CanBeNull] IService newValue);

        public event Action<ServiceLocator> OnDispose;

        /// <summary>
        /// Default shared service locator. Only use this for services that should be registered within the global scope of the application.
        /// </summary>
        [NotNull]
        public static readonly ServiceLocator Shared = new ServiceLocator(nameof(Shared), false);

        private static readonly Dictionary<Type, Func<IService>> DefaultCreateCallbacks = new Dictionary<Type, Func<IService>>();

        /// <summary>
        /// If true and a service is not found, it will try to find the service in the <see cref="Shared"/> instance.
        /// </summary>
        public readonly bool AllowFallbackToShared;

        /// <summary>
        /// If true, it will throw an <see cref="ArgumentOutOfRangeException"/> on any API used with non-interface type arguments.
        /// </summary>
        public readonly bool AllowInterfacesOnly;

        private readonly Dictionary<Type, Service> _services = new Dictionary<Type, Service>();

        /// <param name="id">Identifier that can be used to debugging same <see cref="IService"/> across different <see cref="ServiceLocator"/>.</param>
        /// <param name="allowFallbackToShared">If true and a service is not found, it will try to find the service in the <see cref="Shared"/> instance.</param>
        /// <param name="allowInterfacesOnly">If true, it will throw an <see cref="ArgumentOutOfRangeException"/> on any API used with non-interface type arguments.</param>
        public ServiceLocator(string id, bool allowFallbackToShared = true, bool allowInterfacesOnly = true)
        {
            AllowFallbackToShared = allowFallbackToShared;
            AllowInterfacesOnly = allowInterfacesOnly;
            _services.Clear();
        }

        /// <summary>
        /// Identifier that can be used to debugging same <see cref="IService"/> across different <see cref="ServiceLocator"/>.
        /// </summary>
        [field: SerializeField]
        public string Id { get; private set; }

        /// <summary>
        /// Sets the default callback for when a service needs to be created.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        public static void SetDefaultCreateCallback<T>([CanBeNull] Func<T> createCallback, bool overrideExisting, bool allowInterfacesOnly = true)
            where T : class, IService
        {
            CheckType(allowInterfacesOnly, typeof(T));

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
        [Pure]
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

                if (service.ResetCreateCallbackOnSet)
                {
                    service.CreateCallback = null;
                }

                value = service.Value as T;
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
        /// Gets if the create callback is currently set for a service type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>False if the service has no create callback set or the service type is not found.</returns>
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
        public bool HasCreateCallback([NotNull] Type type)
        {
            Debug.Assert(typeof(IService).IsAssignableFrom(type));
            CheckType(type);

            return _services.TryGetValue(type, out Service service) && service is { CreateCallback: { } };
        }

        /// <summary>
        /// Gets if the service is created by its type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>False if the service wasn't created or the service type is not found.</returns>
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
        public bool IsCreated([NotNull] Type type)
        {
            Debug.Assert(typeof(IService).IsAssignableFrom(type));
            CheckType(type);

            return _services.TryGetValue(type, out Service service) && service is { Value: { } };
        }

        /// <summary>
        /// Removes a listener for when a service instance changes.
        /// </summary>
        /// <param name="callback">The callback to be removed.</param>
        /// <typeparam name="T">The service type.</typeparam>
        public void RemoveValueChangedListener<T>([NotNull] ValueChangeEventHandler callback)
            where T : class, IService
        {
            CheckType(typeof(T));

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
        /// <exception cref="InvalidOperationException">If value already have an <see cref="IService.OwningLocator"/> set.</exception>
        public void Set<T>([CanBeNull] T value)
            where T : class, IService
        {
            Initialize(typeof(T), out Service service);

            if (value is { OwningLocator: { } } && value.OwningLocator != this)
            {
                throw new InvalidOperationException($"The same service can't belong to different {nameof(ServiceLocator)}s!");
            }

            T oldValue = (service.Value as T).GetValid();
            service.Value = value.GetValid();

            if (service.ResetCreateCallbackOnSet)
            {
                service.CreateCallback = null;
            }

            if (oldValue.IsValid())
            {
                oldValue.OwningLocator = null;
            }

            service.ValueChangedCallback(oldValue, value);
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
        [Pure]
        public bool TryGet<T>([CanBeNull] out T value)
            where T : class, IService
        {
            value = Get<T>();

            return value != null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CheckType(bool allowInterfacesOnly, Type type, [CallerMemberName] string memberName = null)
        {
            if (type == typeof(IService))
            {
                throw new ArgumentOutOfRangeException($"{nameof(ServiceLocator)}.{memberName} should not receive IService directly as the type argument!");
            }

            if (allowInterfacesOnly && !type.IsInterface)
            {
                throw new ArgumentOutOfRangeException($"The target {nameof(ServiceLocator)}.{memberName} requires an interface as the type argument!");
            }

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckType(Type type, [CallerMemberName] string memberName = null)
        {
            CheckType(AllowInterfacesOnly, type, memberName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Initialize(Type type, out Service service, [CallerMemberName] string memberName = null)
        {
            CheckType(type, memberName);

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
