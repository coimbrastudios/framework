namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Invoked during <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.LateUpdate.html">LateUpdate</a>.
    /// </summary>
    /// <seealso cref="IPlayerLoopEvent"/>
    public readonly partial struct LateUpdateEvent : IPlayerLoopEvent
    {
        public LateUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }

        /// <inheritdoc/>
        public float DeltaTime { get; }
    }
}
