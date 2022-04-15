using Coimbra.Services.Events;

namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.PostLateUpdate"/>.
    /// </summary>
    public readonly partial struct LastPostLateUpdateEvent : IPlayerLoopEvent, IEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public LastPostLateUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
