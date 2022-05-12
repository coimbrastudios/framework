namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Responsible for any <see cref="IPlayerLoopEvent"/>.
    /// </summary>
    public interface IPlayerLoopService : IService
    {
        /// <summary>
        /// Removes all listeners from all <see cref="IPlayerLoopEvent"/>.
        /// </summary>
        void RemoveAllListeners();
    }
}
