namespace Coimbra.Services
{
    /// <summary>
    /// Invoked during <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.LateUpdate.html">LateUpdate</a>.
    /// </summary>
    public readonly struct LateUpdateEvent : IApplicationLoopEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public LateUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
