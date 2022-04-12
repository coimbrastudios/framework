namespace Coimbra.Services
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.EarlyUpdate"/>.
    /// </summary>
    public readonly struct LastEarlyUpdateEvent : IApplicationLoopEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public LastEarlyUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
