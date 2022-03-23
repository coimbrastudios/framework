namespace Coimbra
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.EarlyUpdate"/>.
    /// </summary>
    public readonly struct LastEarlyUpdateEvent : IPlayerLoopEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public LastEarlyUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
