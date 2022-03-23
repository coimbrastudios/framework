namespace Coimbra
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.PreUpdate"/>.
    /// </summary>
    public readonly struct FirstPreUpdateEvent : IPlayerLoopEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public FirstPreUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
