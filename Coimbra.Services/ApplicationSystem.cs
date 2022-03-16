using UnityEngine;

namespace Coimbra.Services
{
    /// <summary>
    /// Responsible for invoking the Unity's application callbacks.
    /// <seealso cref="ApplicationFocusEvent"/>
    /// <seealso cref="ApplicationPauseEvent"/>
    /// <seealso cref="ApplicationQuitEvent"/>
    /// </summary>
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public class ApplicationSystem : MonoBehaviour
    {
        protected void OnApplicationFocus(bool hasFocus)
        {
            ServiceLocator.Shared.Get<IEventService>()?.Invoke(this, new ApplicationFocusEvent
            {
                IsFocused = hasFocus,
            }, null, true);
        }

        protected void OnApplicationPause(bool pauseStatus)
        {
            ServiceLocator.Shared.Get<IEventService>()?.Invoke(this, new ApplicationPauseEvent
            {
                IsPaused = pauseStatus,
            }, null, true);
        }

        protected void OnApplicationQuit()
        {
            ServiceLocator.Shared.Get<IEventService>()?.Invoke(this, new ApplicationQuitEvent(), null, true);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            GameObject gameObject = new GameObject(nameof(ApplicationSystem), typeof(ApplicationSystem))
            {
                hideFlags = HideFlags.HideAndDontSave,
            };

            DontDestroyOnLoad(gameObject);
        }
    }
}
