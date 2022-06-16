using Coimbra.Services.Events;

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
        PlayerLoopTimingEvents CurrentTimings { get; set; }

        /// <inheritdoc cref="IEventService.AddListener{TEvent}"/>
        EventHandle AddListener(SerializableType<IPlayerLoopEvent> eventType, PlayerLoopEventHandler eventHandler);

        /// <summary>
        /// Removes all listeners from the specified type.
        /// </summary>
        void RemoveAllListeners<T>()
            where T : IPlayerLoopEvent;
    }
}
