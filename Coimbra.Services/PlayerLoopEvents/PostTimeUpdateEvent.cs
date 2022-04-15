using Coimbra.Services.Events;

namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.TimeUpdate"/>.
    /// </summary>
    public readonly partial struct PostTimeUpdateEvent : IPlayerLoopEvent, IEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public PostTimeUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
