namespace Coimbra.Services
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.TimeUpdate"/>.
    /// </summary>
    public readonly struct PreTimeUpdateEvent : IApplicationLoopEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public PreTimeUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
