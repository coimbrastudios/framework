using UnityEngine;
using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Provides easy access to application-wise Unity callbacks.
    /// </summary>
    [RequireImplementors]
    public interface IApplicationService
    {
        delegate void FocusHandler(bool isFocused);

        delegate void PauseHandler(bool isPaused);

        delegate void QuitHandler();

        /// <summary>
        /// Works the same as <see cref="MonoBehaviour"/>.OnApplicationFocus.
        /// </summary>
        event FocusHandler OnFocus;

        /// <summary>
        /// Works the same as <see cref="MonoBehaviour"/>.OnApplicationPause.
        /// </summary>
        event PauseHandler OnPause;

        /// <summary>
        /// Works the same as <see cref="MonoBehaviour"/>.OnApplicationQuit.
        /// </summary>
        event QuitHandler OnQuit;

        /// <summary>
        /// Resets the OnFocus event.
        /// </summary>
        void ResetFocusEvent();

        /// <summary>
        /// Resets the OnPause event.
        /// </summary>
        void ResetPauseEvent();

        /// <summary>
        /// Resets the OnQuit event.
        /// </summary>
        void ResetQuitEvent();
    }
}
