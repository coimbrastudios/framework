namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.TimeUpdate"/>.
    /// </summary>
    /// <seealso cref="IPlayerLoopEvent"/>
    public readonly partial struct PostTimeUpdateEvent : IPlayerLoopEvent
    {
        public PostTimeUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }

        /// <inheritdoc/>
        public float DeltaTime { get; }
    }
}
