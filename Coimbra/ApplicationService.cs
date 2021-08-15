using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Provides easy access to application-wise Unity callbacks.
    /// </summary>
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

    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    internal sealed class ApplicationService : MonoBehaviour, IApplicationService
    {
        public event IApplicationService.FocusHandler OnFocus
        {
            add => _focusCallback += value;
            remove => _focusCallback -= value;
        }

        public event IApplicationService.PauseHandler OnPause
        {
            add => _pauseCallback += value;
            remove => _pauseCallback -= value;
        }

        public event IApplicationService.QuitHandler OnQuit
        {
            add => _quitCallback += value;
            remove => _quitCallback -= value;
        }

        private IApplicationService.FocusHandler _focusCallback;

        private IApplicationService.PauseHandler _pauseCallback;

        private IApplicationService.QuitHandler _quitCallback;

        public void ResetFocusEvent()
        {
            _focusCallback = null;
        }

        public void ResetPauseEvent()
        {
            _pauseCallback = null;
        }

        public void ResetQuitEvent()
        {
            _quitCallback = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            if (ServiceLocator.Global.IsCreated<IApplicationService>() || ServiceLocator.Global.HasCreateCallback<IApplicationService>())
            {
                return;
            }

            ServiceLocator.Global.SetCreateCallback(Create, true);
        }

        private static IApplicationService Create()
        {
            GameObject gameObject = new GameObject(nameof(ApplicationService));
            DontDestroyOnLoad(gameObject);

            return gameObject.AddComponent<ApplicationService>();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            _focusCallback?.Invoke(hasFocus);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            _pauseCallback?.Invoke(pauseStatus);
        }

        private void OnApplicationQuit()
        {
            _quitCallback?.Invoke();
        }
    }
}
