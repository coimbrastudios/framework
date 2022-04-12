namespace Coimbra.Services
{
    /// <summary>
    /// Invoked during <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html">Update</a>.
    /// </summary>
    public readonly struct UpdateEvent : IApplicationLoopEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public UpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
