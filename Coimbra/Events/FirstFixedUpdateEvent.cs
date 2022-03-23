namespace Coimbra
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.FixedUpdate"/>.
    /// </summary>
    public readonly struct FirstFixedUpdateEvent : IPlayerLoopEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public FirstFixedUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
