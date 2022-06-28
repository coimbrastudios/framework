namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.PostLateUpdate"/>.
    /// </summary>
    public readonly partial struct PostLateUpdateEvent : IPlayerLoopEvent
    {
        public PostLateUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }

        /// <inheritdoc/>
        public float DeltaTime { get; }
    }
}
