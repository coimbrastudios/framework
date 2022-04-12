namespace Coimbra.Services
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.Initialization"/>.
    /// </summary>
    public readonly struct FirstInitializationEvent : IApplicationLoopEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public FirstInitializationEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
