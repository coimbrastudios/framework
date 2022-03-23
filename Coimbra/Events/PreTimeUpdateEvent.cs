namespace Coimbra
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.TimeUpdate"/>.
    /// </summary>
    public readonly struct PreTimeUpdateEvent : IPlayerLoopEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public PreTimeUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
