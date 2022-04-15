using Coimbra.Services.Events;

namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Invoked during <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.FixedUpdate.html">FixedUpdate</a>.
    /// </summary>
    public readonly partial struct FixedUpdateEvent : IPlayerLoopEvent, IEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public FixedUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
