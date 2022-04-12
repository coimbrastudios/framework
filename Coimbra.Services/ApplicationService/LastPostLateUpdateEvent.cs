namespace Coimbra.Services
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.PostLateUpdate"/>.
    /// </summary>
    public readonly struct LastPostLateUpdateEvent : IApplicationLoopEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public LastPostLateUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
