#nullable enable

namespace Coimbra.Services
{
    /// <summary>
    /// Responsible for creating a new service on-demand.
    /// </summary>
    /// <remarks>
    /// Allows to define how the <see cref="ServiceLocator.Get{T}"/> API should behave when <see cref="ServiceLocator.IsSet{T}()"/> is false.
    /// Requires to be registered with <see cref="ServiceLocator.SetFactory{T}"/> for each <see cref="IService"/> type, usually during <see cref="UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration"/>.
    /// </remarks>
    /// <seealso cref="DisableDefaultFactoryAttribute"/>
    /// <seealso cref="DefaultServiceActorFactory{T}"/>
    /// <seealso cref="DefaultServiceFactory{T}"/>
    /// <seealso cref="IService"/>
    /// <seealso cref="ServiceLocator"/>
    public interface IServiceFactory
    {
        /// <summary>
        /// Gets a value indicating whether the value returned by <see cref="GetService"/> should be used to set the current service instance or if it should be treated as a temporary value.
        /// </summary>
        bool ShouldSetService => true;

        /// <summary>
        /// Implement this to determine which service instance should be returned.
        /// </summary>
        IService? GetService();
    }
}
