namespace Coimbra.Services
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.PreLateUpdate"/>.
    /// </summary>
    public readonly struct PreLateUpdateEvent : IApplicationLoopEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public PreLateUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
