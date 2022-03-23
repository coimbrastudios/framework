namespace Coimbra
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.Update"/>.
    /// </summary>
    public readonly struct FirstUpdateEvent : IPlayerLoopEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public FirstUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
