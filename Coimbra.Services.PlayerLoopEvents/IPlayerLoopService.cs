using Cysharp.Threading.Tasks;

namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Responsible for any <see cref="IPlayerLoopEvent"/>.
    /// </summary>
    public interface IPlayerLoopService : IService
    {
        /// <summary>
        /// The list of events currently being fired.
        /// </summary>
        InjectPlayerLoopTimings CurrentTimings { get; set; }

        /// <summary>
        /// Removes all listeners from the specified type.
        /// </summary>
        void RemoveAllListeners<T>()
            where T : IPlayerLoopEvent;
    }
}
