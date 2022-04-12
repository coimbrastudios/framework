namespace Coimbra.Services
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.FixedUpdate"/>.
    /// </summary>
    public readonly struct FirstFixedUpdateEvent : IApplicationLoopEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public FirstFixedUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
