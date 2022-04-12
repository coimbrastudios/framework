namespace Coimbra.Services
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.FixedUpdate"/>.
    /// </summary>
    public readonly struct LastFixedUpdateEvent : IApplicationLoopEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public LastFixedUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
