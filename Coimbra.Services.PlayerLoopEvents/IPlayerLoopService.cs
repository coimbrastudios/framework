using Coimbra.Services.Events;

namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Responsible for any <see cref="IPlayerLoopEvent"/>, which includes <see cref="UpdateEvent"/>, <see cref="LateUpdateEvent"/>, <see cref="FixedUpdateEvent"/>, and each <see cref="PlayerLoopInjectedTimings"/>.
    /// </summary>
    [RequiredService]
    public interface IPlayerLoopService : IService
    {
        /// <summary>
        /// Gets or sets the list of events currently being fired.
        /// </summary>
        PlayerLoopInjectedTimings CurrentInjectedTimings { get; set; }

        /// <inheritdoc cref="IEventService.AddListener{TEvent}"/>
        EventHandle AddListener(SerializableType<IPlayerLoopEvent> eventType, PlayerLoopEventHandler eventHandler);

        /// <inheritdoc cref="IEventService.RemoveAllListeners{TEvent}"/>
        bool RemoveAllListeners<T>()
            where T : IPlayerLoopEvent;
    }
}
