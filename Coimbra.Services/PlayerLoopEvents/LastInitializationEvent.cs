using Coimbra.Services.Events;

namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.Initialization"/>.
    /// </summary>
    public readonly partial struct LastInitializationEvent : IPlayerLoopEvent, IEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public LastInitializationEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
