using Coimbra.Services.Events;

namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Invoked during <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html">Update</a>.
    /// </summary>
    public readonly partial struct UpdateEvent : IPlayerLoopEvent, IEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public UpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
