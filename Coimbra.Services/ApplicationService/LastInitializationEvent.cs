namespace Coimbra.Services
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.Initialization"/>.
    /// </summary>
    public readonly struct LastInitializationEvent : IApplicationLoopEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public LastInitializationEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
