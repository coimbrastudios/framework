using Coimbra.Services.Events;

namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.PostLateUpdate"/>.
    /// </summary>
    public readonly partial struct PostLateUpdateEvent : IPlayerLoopEvent, IEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public PostLateUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
