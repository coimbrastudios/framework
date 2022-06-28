namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.Initialization"/>.
    /// </summary>
    public readonly partial struct LastInitializationEvent : IPlayerLoopEvent
    {
        public LastInitializationEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }

        /// <inheritdoc/>
        public float DeltaTime { get; }
    }
}
