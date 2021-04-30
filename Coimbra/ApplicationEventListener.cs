using UnityEngine;

namespace Coimbra
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public sealed class ApplicationEventListener : MonoBehaviour
    {
        public delegate void FocusHandler(bool isFocused);

        public delegate void PauseHandler(bool isPaused);

        public delegate void QuitHandler();

        public static event FocusHandler OnFocus
        {
            add => Instance._onFocus += value;
            remove => Instance._onFocus -= value;
        }

        public static event PauseHandler OnPause
        {
            add => Instance._onPause += value;
            remove => Instance._onPause -= value;
        }

        public static event QuitHandler OnQuit
        {
            add => Instance._onQuit += value;
            remove => Instance._onQuit -= value;
        }

        private static ApplicationEventListener _instanceBackingField;
        private FocusHandler _onFocus;
        private PauseHandler _onPause;
        private QuitHandler _onQuit;

        private static ApplicationEventListener Instance
        {
            get
            {
                if (_instanceBackingField == null)
                {
                    GameObject gameObject = new GameObject(nameof(ApplicationEventListener));
                    _instanceBackingField = gameObject.AddComponent<ApplicationEventListener>();
                    DontDestroyOnLoad(gameObject);
                }

                return _instanceBackingField;
            }
        }

        public static void ResetFocusEvent()
        {
            Instance._onFocus = null;
        }

        public static void ResetPauseEvent()
        {
            Instance._onPause = null;
        }

        public static void ResetQuitEvent()
        {
            Instance._onQuit = null;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            _onFocus?.Invoke(hasFocus);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            _onPause?.Invoke(pauseStatus);
        }

        private void OnApplicationQuit()
        {
            _onQuit?.Invoke();
        }
    }
}
