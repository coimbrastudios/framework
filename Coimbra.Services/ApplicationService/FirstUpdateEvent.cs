namespace Coimbra.Services
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.Update"/>.
    /// </summary>
    public readonly struct FirstUpdateEvent : IApplicationLoopEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public FirstUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
