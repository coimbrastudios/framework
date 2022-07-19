#nullable enable

using UnityEngine;

namespace Coimbra.Services
{
    /// <summary>
    /// The default factory for a service that inherits from <see cref="Actor"/>.
    /// </summary>
    public sealed class DefaultServiceActorFactory<T> : IServiceFactory
        where T : Actor, IService
    {
        public static readonly DefaultServiceActorFactory<T> Instance = new();

        private DefaultServiceActorFactory() { }

        /// <inheritdoc/>
        public IService GetService()
        {
            return Object.FindObjectOfType<T>().TryGetValid(out T value) ? value : new GameObject(typeof(T).Name).AsActor<T>()!;
        }
    }
}
