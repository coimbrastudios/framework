namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Invoked during <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.FixedUpdate.html">FixedUpdate</a>.
    /// </summary>
    public readonly partial struct FixedUpdateEvent : IPlayerLoopEvent
    {
        public FixedUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }

        /// <inheritdoc/>
        public float DeltaTime { get; }
    }
}
