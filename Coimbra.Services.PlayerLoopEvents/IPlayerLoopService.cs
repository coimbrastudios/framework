using Coimbra.Services.Events;

namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Responsible for any <see cref="IPlayerLoopEvent"/>, which includes <see cref="UpdateEvent"/>, <see cref="LateUpdateEvent"/>, <see cref="FixedUpdateEvent"/>, and each <see cref="PlayerLoopInjectedTimings"/>.
    /// </summary>
    /// <seealso cref="PlayerLoopSystem"/>
    /// <seealso cref="PlayerLoopEventListener"/>
    /// <seealso cref="IPlayerLoopEvent"/>
    /// <seealso cref="FixedUpdateEvent"/>
    /// <seealso cref="UpdateEvent"/>
    /// <seealso cref="LateUpdateEvent"/>
    /// <seealso cref="FirstInitializationEvent"/>
    /// <seealso cref="LastInitializationEvent"/>
    /// <seealso cref="FirstEarlyUpdateEvent"/>
    /// <seealso cref="LastEarlyUpdateEvent"/>
    /// <seealso cref="FirstFixedUpdateEvent"/>
    /// <seealso cref="LastFixedUpdateEvent"/>
    /// <seealso cref="FirstPreUpdateEvent"/>
    /// <seealso cref="LastPreUpdateEvent"/>
    /// <seealso cref="FirstUpdateEvent"/>
    /// <seealso cref="LastUpdateEvent"/>
    /// <seealso cref="PreLateUpdateEvent"/>
    /// <seealso cref="FirstPostLateUpdateEvent"/>
    /// <seealso cref="PostLateUpdateEvent"/>
    /// <seealso cref="LastPostLateUpdateEvent"/>
    /// <seealso cref="PreTimeUpdateEvent"/>
    /// <seealso cref="PostTimeUpdateEvent"/>
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
