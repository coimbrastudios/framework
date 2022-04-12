namespace Coimbra.Services
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.EarlyUpdate"/>.
    /// </summary>
    public readonly struct FirstEarlyUpdateEvent : IApplicationLoopEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public FirstEarlyUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
