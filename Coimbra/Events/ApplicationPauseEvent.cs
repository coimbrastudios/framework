using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Invoked during <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnApplicationPause.html">OnApplicationPause</a>.
    /// </summary>
    [Preserve]
    public readonly struct ApplicationPauseEvent
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
