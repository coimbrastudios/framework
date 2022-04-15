using Coimbra.Services.Events;

namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.EarlyUpdate"/>.
    /// </summary>
    public readonly partial struct LastEarlyUpdateEvent : IPlayerLoopEvent, IEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public LastEarlyUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
