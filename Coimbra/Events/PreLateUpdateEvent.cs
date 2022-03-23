namespace Coimbra
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.PreLateUpdate"/>.
    /// </summary>
    public readonly struct PreLateUpdateEvent : IPlayerLoopEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public PreLateUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
