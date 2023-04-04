#nullable enable

using UnityEngine;

namespace Coimbra.Services
{
    /// <summary>
    /// The default factory for any <see cref="IService"/> implementation class that inherits from <see cref="Actor"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="GetService"/> will first try to find an existing instance with <see cref="Object.FindObjectOfType{T}()"/>. If none is found it will create a new <see cref="GameObject"/> for the given <see cref="Actor"/> type and initialize it.
    /// </remarks>
    /// <seealso cref="DisableDefaultFactoryAttribute"/>
    /// <seealso cref="DefaultServiceFactory{T}"/>
    /// <seealso cref="IService"/>
    /// <seealso cref="IServiceFactory"/>
    /// <seealso cref="ServiceLocator"/>
    public sealed class DefaultServiceActorFactory<T> : IServiceFactory
        where T : Actor, IService
    {
        /// <summary>
        /// Cached instance of the factory as it is stateless.
        /// </summary>
        public static readonly DefaultServiceActorFactory<T> Instance = new();

        private DefaultServiceActorFactory() { }

        /// <summary>
        /// Tries to find an existing loaded instance and fallbacks to creating a new <see cref="Actor"/> of the given type.
        /// </summary>
        /// <returns>
        /// Any existing loaded instance or a new <see cref="Actor"/> of the given type.
        /// </returns>
        public IService GetService()
        {
            return Object.FindAnyObjectByType<T>().TryGetValid(out T value) ? value : new GameObject(typeof(T).Name).GetOrInitializeActor<T>()!;
        }
    }
}
