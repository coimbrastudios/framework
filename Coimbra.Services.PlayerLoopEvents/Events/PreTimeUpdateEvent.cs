namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.TimeUpdate"/>.
    /// </summary>
    public readonly partial struct PreTimeUpdateEvent : IPlayerLoopEvent
    {
        public PreTimeUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }

        /// <inheritdoc/>
        public float DeltaTime { get; }
    }
}
