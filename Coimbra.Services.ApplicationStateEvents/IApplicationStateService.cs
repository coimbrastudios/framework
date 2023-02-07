using UnityEngine.Scripting;

namespace Coimbra.Services.ApplicationStateEvents
{
    /// <summary>
    /// Responsible for part of the application lifetime cycle, meant to be used to fire Unity events related to quit, pause, and focus.
    /// </summary>
    /// <remarks>
    /// Meant to replace usage of <see cref="UnityEngine.MonoBehaviour"/> when listening to <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnApplicationFocus.html">OnApplicationFocus</a>, <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnApplicationPause.html">OnApplicationPause</a>, and <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnApplicationQuit.html">OnApplicationQuit</a>.
    /// </remarks>
    /// <seealso cref="IApplicationStateEvent"/>
    /// <seealso cref="ApplicationFocusEvent"/>
    /// <seealso cref="ApplicationPauseEvent"/>
    /// <seealso cref="ApplicationQuitEvent"/>
    /// <seealso cref="ApplicationStateSystem"/>
    [RequiredService]
    [RequireImplementors]
    public interface IApplicationStateService : IService
    {
        /// <summary>
        /// Gets a value indicating whether application is focused.
        /// </summary>
        bool IsFocused { get; }

        /// <summary>
        /// Gets a value indicating whether application is paused.
        /// </summary>
        bool IsPaused { get; }

        /// <summary>
        /// Gets a value indicating whether application is quitting.
        /// </summary>
        bool IsQuitting { get; }

        /// <summary>
        /// Removes all listeners from the specified type.
        /// </summary>
        void RemoveAllListeners<T>()
            where T : IApplicationStateEvent;
    }
}
