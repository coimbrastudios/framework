namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Responsible for any <see cref="IPlayerLoopEvent"/>.
    /// </summary>
    public interface IPlayerLoopService : IService
    {
        /// <summary>
        /// Removes all listeners from the specified type.
        /// </summary>
        void RemoveAllListeners<T>()
            where T : IPlayerLoopEvent;
    }
}
