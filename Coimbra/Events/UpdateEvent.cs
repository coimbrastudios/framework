namespace Coimbra
{
    /// <summary>
    /// Invoked during <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html">Update</a>.
    /// </summary>
    public readonly struct UpdateEvent : IPlayerLoopEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public UpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
