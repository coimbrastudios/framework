#nullable enable

namespace Coimbra.Services
{
    /// <summary>
    /// Responsible for creating a new service on-demand.
    /// </summary>
    public interface IServiceFactory
    {
        /// <summary>
        /// Implements this to determine which service instance should be returned.
        /// </summary>
        IService? GetService();
    }
}
