using Coimbra.Services.Events;

namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.PreUpdate"/>.
    /// </summary>
    public readonly partial struct FirstPreUpdateEvent : IPlayerLoopEvent, IEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public FirstPreUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
