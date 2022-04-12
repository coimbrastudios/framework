namespace Coimbra.Services
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.PreUpdate"/>.
    /// </summary>
    public readonly struct FirstPreUpdateEvent : IApplicationLoopEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public FirstPreUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
