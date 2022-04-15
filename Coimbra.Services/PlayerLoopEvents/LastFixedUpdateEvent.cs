using Coimbra.Services.Events;

namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.FixedUpdate"/>.
    /// </summary>
    public readonly partial struct LastFixedUpdateEvent : IPlayerLoopEvent, IEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public LastFixedUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
