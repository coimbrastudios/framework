using Coimbra.Services.Events;

namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.PreUpdate"/>.
    /// </summary>
    public readonly partial struct LastPreUpdateEvent : IPlayerLoopEvent, IEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public LastPreUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
