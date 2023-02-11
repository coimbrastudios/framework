#nullable enable

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;
using Debug = UnityEngine.Debug;

namespace Coimbra.Services
{
    /// <summary>
    /// A non-thread-safe service locator implementation.
    /// </summary>
    /// <remarks>
    /// This implementation comes with some Roslyn Analyzers to ensure that the APIs are being used as designed, so only interfaces that extends <see cref="IService"/> can be used as type parameters in the APIs.
    /// Any attempt to use classes or structs as type parameters will result in compile-time errors, avoiding runtime issues.
    /// <para></para>
    /// You can create your custom <see cref="IServiceFactory"/> so that you can inject your own logic for when <see cref="Get{T}"/>, <see cref="TryGet{T}"/>, or <see cref="GetChecked{T}"/> is used.
    /// <para></para>
    /// You can also listen for when a service is set with <see cref="AddSetListener{T}"/>, which will be fired even if it was set due a custom <see cref="IServiceFactory"/> implementation.
    /// <para></para>
    /// It also fully supports the new <b>Enter Play Mode Options</b> (any combination) and offers a debug window at <b>Window/Coimbra Framework/Service Locator</b>.
    /// <para></para>
    /// In most cases, you can simply register the <see cref="IServiceFactory"/> during application startup (i.e. on <see cref="UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration"/>) and then let the services be created on-demand.
    /// Doing this will minimize the need to keep track of all dependencies between different services, but might create hard-to-track bugs if you end up having cyclic dependencies.
    /// To avoid this situation, it is highly suggested to divide your application into smaller assemblies and having one assembly per service implementation, as assemblies already emit compile-time errors for cyclic references.
    /// <para></para>
    /// If having a more refined control over initialization logic is required you can still manually call <see cref="Set{T}"/> for each of your services, or even mix this with the solution above if you only need this level of control in a few places.
    /// </remarks>
    /// <seealso cref="IService"/>
    /// <seealso cref="IServiceFactory"/>
    [Preserve]
    public static class ServiceLocator
    {
        internal sealed class Service
        {
            internal readonly bool IsDynamic;

            internal readonly bool IsRequired;

            internal Service(Type type)
            {
                IsDynamic = type.GetCustomAttribute<DynamicServiceAttribute>() != null;
                IsRequired = type.GetCustomAttribute<RequiredServiceAttribute>() != null;
            }

            internal IService? Value { get; set; }

            internal IServiceFactory? Factory { get; set; }

            internal SetHandler? SetHandler { get; set; }
        }

        /// <summary>
        /// Delegate to use with <see cref="ServiceLocator.AddSetListener{T}"/> and <see cref="ServiceLocator.RemoveSetListener{T}"/>.
        /// </summary>
        public delegate void SetHandler(Type service);

        internal static readonly Dictionary<Type, Service> Services = new();

        /// <summary>
        /// Adds a listener for when a service instance changes.
        /// </summary>
        /// <param name="callback">The callback to be invoked.</param>
        /// <typeparam name="T">The service type.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddSetListener<T>(SetHandler callback)
            where T : class, IService
        {
            Initialize(typeof(T), out Service service);

            service.SetHandler += callback;
        }

        /// <summary>
        /// Gets a service instance.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>The service instance.</returns>
        public static T? Get<T>()
            where T : class, IService
        {
            Initialize(typeof(T), out Service service);

            return Get<T>(service);
        }

        /// <summary>
        /// Gets a service instance. It also asserts that both <typeparamref name="T"/> has <see cref="RequiredServiceAttribute"/> and the value is valid.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>The service instance.</returns>
        public static T GetChecked<T>()
            where T : class, IService
        {
            Initialize(typeof(T), out Service service);
            Debug.Assert(service.IsRequired, $"Called {nameof(GetChecked)} but service {typeof(T)} is missing {nameof(RequiredServiceAttribute)}! Use {nameof(Get)} or {nameof(TryGet)} instead.");

            T? value = Get<T>(service);
            Debug.Assert(value.IsValid(), $"Called {nameof(GetChecked)} but service {typeof(T)} was null!");

            return value!;
        }

        /// <summary>
        /// Gets if the factory is set for a service type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>False if the service has no factory set.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasFactory<T>()
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
        public static bool HasFactory(Type type)
        {
            return Services.TryGetValue(type, out Service service) && service is { Factory: { } };
        }

        /// <summary>
        /// Gets if the service is created by its type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>False if the service wasn't created.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSet<T>()
            where T : class, IService
        {
            return IsSet(typeof(T));
        }

        /// <summary>
        /// Gets if the service is created by its type.
        /// </summary>
        /// <param name="type">The service type.</param>
        /// <returns>False if the service wasn't created.</returns>
        [Pure]
        public static bool IsSet(Type type)
        {
            return Services.TryGetValue(type, out Service service) && service.Value != null;
        }

        /// <summary>
        /// Gets if the service is created by its type.
        /// </summary>
        /// <param name="value">The service value, if created.</param>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>False if the service wasn't created.</returns>
        [Pure]
        public static bool IsSet<T>([NotNullWhen(true)] out T? value)
            where T : class, IService
        {
            if (IsSet(typeof(T), out IService? service))
            {
                value = (T)service;

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
        public static bool IsSet(Type type, [NotNullWhen(true)] out IService? value)
        {
            if (Services.TryGetValue(type, out Service service))
            {
                return service.Value.TryGetValid(out value);
            }

            value = null;

            return false;
        }

        /// <summary>
        /// Removes a listener for when a service instance changes.
        /// </summary>
        /// <param name="callback">The callback to be removed.</param>
        /// <typeparam name="T">The service type.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveSetListener<T>(SetHandler callback)
            where T : class, IService
        {
            if (Services.TryGetValue(typeof(T), out Service service))
            {
                service.SetHandler -= callback;
            }
        }

        /// <summary>
        /// Sets a service instance. Will fail if value is already set and <typeparamref name="T"/> is missing <see cref="DynamicAttribute"/>.
        /// </summary>
        /// <param name="value">The service instance.</param>
        /// <typeparam name="T">The service type.</typeparam>
        public static void Set<T>(T? value)
            where T : class, IService
        {
            Initialize(typeof(T), out Service service);

            if (service.Value.TryGetValid(out IService? oldValue))
            {
                if (!service.IsDynamic)
                {
                    Debug.LogError($"Service of type {typeof(T)} was not defined as dynamic but you are trying to override it!");

                    return;
                }

                oldValue.Dispose();
            }

            service.Value = value.GetValid();
            service.SetHandler?.Invoke(typeof(T));
        }

        /// <summary>
        /// Sets the factory for when a service needs to be created.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetFactory<T>(IServiceFactory? factory)
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGet<T>([NotNullWhen(true)] out T? value)
            where T : class, IService
        {
            value = Get<T>();

            return value != null;
        }

        internal static void Reset()
        {
            foreach (Service service in Services.Values)
            {
                service.Factory = null;
                service.SetHandler = null;
                service.Value = service.Value.GetValid();
                service.Value?.Dispose();

                service.Value = null;
            }

            Services.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T? Get<T>(Service service)
            where T : class, IService
        {
            if (service.Value.TryGetValid(out IService? value))
            {
                return (T)value;
            }

            if (service.Factory == null || !service.Factory.GetService().TryGetValid(out value))
            {
                return null;
            }

            switch (value)
            {
                case T result:
                {
                    if (service.Factory.ShouldSetService)
                    {
#pragma warning disable COIMBRA0110
                        Set(result);
#pragma warning restore COIMBRA0110
                    }

                    return result;
                }

                default:
                {
                    Debug.LogWarning($"Create callback for {typeof(T)} returned a service of type {value.GetType()}! Disposing it...");
                    value.Dispose();

                    return null;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Initialize(Type type, out Service service)
        {
            if (Services.TryGetValue(type, out service))
            {
                return;
            }

            service = new Service(type);
            Services[type] = service;
        }
    }
}
