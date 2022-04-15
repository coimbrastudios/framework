using Coimbra.Services.Events;

namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.PreLateUpdate"/>.
    /// </summary>
    public readonly partial struct PreLateUpdateEvent : IPlayerLoopEvent, IEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public PreLateUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
