namespace Coimbra.Services
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.PreUpdate"/>.
    /// </summary>
    public readonly struct LastPreUpdateEvent : IApplicationLoopEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public LastPreUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
