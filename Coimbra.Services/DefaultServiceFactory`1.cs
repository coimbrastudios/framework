#nullable enable

namespace Coimbra.Services
{
    /// <summary>
    /// The default factory for a service with a public parameterless constructor.
    /// </summary>
    public sealed class DefaultServiceFactory<T> : IServiceFactory
        where T : class, IService, new()
    {
        public static readonly DefaultServiceFactory<T> Instance = new();

        private DefaultServiceFactory() { }

        /// <inheritdoc/>
        public IService GetService()
        {
            return new T();
        }
    }
}
