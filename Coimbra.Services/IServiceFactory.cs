#nullable enable

namespace Coimbra.Services
{
    /// <summary>
    /// Responsible for creating a new service on-demand.
    /// </summary>
    public interface IServiceFactory
    {
        /// <summary>
        /// Implements this to determine how the service should be create.
        /// </summary>
        IService? Create();
    }
}
