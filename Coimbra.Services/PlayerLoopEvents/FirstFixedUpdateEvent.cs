using Coimbra.Services.Events;

namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.FixedUpdate"/>.
    /// </summary>
    public readonly partial struct FirstFixedUpdateEvent : IPlayerLoopEvent, IEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public FirstFixedUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
