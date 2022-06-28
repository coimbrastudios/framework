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
        public static readonly DefaultServiceActorFactory<T> Instance = new DefaultServiceActorFactory<T>();

        private DefaultServiceActorFactory() { }

        /// <inheritdoc/>
        public IService Create()
        {
            return new GameObject(typeof(T).Name).AsActor<T>()!;
        }
    }
}
