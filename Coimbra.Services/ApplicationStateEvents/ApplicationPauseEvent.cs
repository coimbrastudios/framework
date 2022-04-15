using Coimbra.Services.Events;

namespace Coimbra.Services.ApplicationStateEvents
{
    /// <summary>
    /// Invoked during <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnApplicationPause.html">OnApplicationPause</a>.
    /// </summary>
    public readonly partial struct ApplicationPauseEvent : IApplicationStateEvent, IEvent
    {
        /// <summary>
        /// True if currently paused.
        /// </summary>
        public readonly bool IsPaused;

        /// <param name="isPaused">True if currently paused.</param>
        public ApplicationPauseEvent(bool isPaused)
        {
            IsPaused = isPaused;
        }
    }
}
