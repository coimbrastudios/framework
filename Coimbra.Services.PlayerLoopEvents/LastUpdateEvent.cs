namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.Update"/>.
    /// </summary>
    public readonly partial struct LastUpdateEvent : IPlayerLoopEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public LastUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
