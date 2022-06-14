#nullable enable

namespace Coimbra.Services
{
    /// <summary>
    /// The default factory for a service with a public parameterless constructor.
    /// </summary>
    public sealed class DefaultServiceFactory<T> : IServiceFactory
        where T : class, IService, new()
    {
        public static readonly DefaultServiceFactory<T> Instance = new DefaultServiceFactory<T>();

        private DefaultServiceFactory() { }

        /// <inheritdoc/>
        public IService Create()
        {
            return new T();
        }
    }
}
