using Coimbra.Services.Events;

namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.Initialization"/>.
    /// </summary>
    public readonly partial struct FirstInitializationEvent : IPlayerLoopEvent, IEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public FirstInitializationEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
