namespace Coimbra.Services
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.Update"/>.
    /// </summary>
    public readonly struct LastUpdateEvent : IApplicationLoopEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public LastUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
