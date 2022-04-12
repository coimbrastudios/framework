namespace Coimbra.Services
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.TimeUpdate"/>.
    /// </summary>
    public readonly struct PostTimeUpdateEvent : IApplicationLoopEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public PostTimeUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
