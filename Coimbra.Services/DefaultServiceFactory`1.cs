#nullable enable

namespace Coimbra.Services
{
    /// <summary>
    /// The default factory for any <see cref="IService"/> implementation class with a public parameterless constructor.
    /// </summary>
    /// <remarks>
    /// <see cref="GetService"/> will simple create a new instance of the class using the default parameterless constructor.
    /// </remarks>
    /// <seealso cref="DisableDefaultFactoryAttribute"/>
    /// <seealso cref="DefaultServiceActorFactory{T}"/>
    /// <seealso cref="IService"/>
    /// <seealso cref="IServiceFactory"/>
    /// <seealso cref="ServiceLocator"/>
    public sealed class DefaultServiceFactory<T> : IServiceFactory
        where T : class, IService, new()
    {
        /// <summary>
        /// Cached instance of the factory as it is stateless.
        /// </summary>
        public static readonly DefaultServiceFactory<T> Instance = new();

        private DefaultServiceFactory() { }

        /// <summary>
        /// Creates a new instance of the given type using its parameterless constructor.
        /// </summary>
        /// <returns>A new instance of the given type.</returns>
        public IService GetService()
        {
            return new T();
        }
    }
}
