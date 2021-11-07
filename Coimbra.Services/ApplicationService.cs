using UnityEngine;

namespace Coimbra.Services
{
    /// <summary>
    /// Default implementation for <see cref="IApplicationService"/>.
    /// </summary>
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public class ApplicationService : MonoBehaviour, IApplicationService
    {
        /// <inheritdoc cref="IApplicationService.OnFocus"/>>
        public event IApplicationService.FocusHandler OnFocus
        {
            add => _focusCallback += value;
            remove => _focusCallback -= value;
        }

        /// <inheritdoc cref="IApplicationService.OnPause"/>>
        public event IApplicationService.PauseHandler OnPause
        {
            add => _pauseCallback += value;
            remove => _pauseCallback -= value;
        }

        /// <inheritdoc cref="IApplicationService.OnQuit"/>>
        public event IApplicationService.QuitHandler OnQuit
        {
            add => _quitCallback += value;
            remove => _quitCallback -= value;
        }

        private IApplicationService.FocusHandler _focusCallback;
        private IApplicationService.PauseHandler _pauseCallback;
        private IApplicationService.QuitHandler _quitCallback;

        /// <inheritdoc cref="IApplicationService.ResetFocusEvent"/>>
        public void ResetFocusEvent()
        {
            _focusCallback = null;
        }

        /// <inheritdoc cref="IApplicationService.ResetPauseEvent"/>>
        public void ResetPauseEvent()
        {
            _pauseCallback = null;
        }

        /// <inheritdoc cref="IApplicationService.ResetQuitEvent"/>>
        public void ResetQuitEvent()
        {
            _quitCallback = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            ServiceLocator.SetDefaultCreateCallback(Create, false);
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
