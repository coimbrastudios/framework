using UnityEngine;

namespace Coimbra.Systems
{
    /// <summary>
    /// Responsible for the application lifetime cycle and fires Unity's application callbacks.
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
            ServiceLocator.Shared.Get<IEventService>()?.Invoke(this, new ApplicationFocusEvent(hasFocus), null, true);
        }

        protected void OnApplicationPause(bool pauseStatus)
        {
            ServiceLocator.Shared.Get<IEventService>()?.Invoke(this, new ApplicationPauseEvent(pauseStatus), null, true);
        }

        protected void OnApplicationQuit()
        {
            ServiceLocator.Shared.Get<IEventService>()?.Invoke(this, new ApplicationQuitEvent(), null, true);

#if UNITY_EDITOR
            ServiceLocator.Shared.Dispose();
#endif
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            GameObject gameObject = new GameObject(nameof(ApplicationSystem), typeof(ApplicationSystem))
            {
                hideFlags = HideFlags.NotEditable,
            };

            DontDestroyOnLoad(gameObject);
        }
    }
}
