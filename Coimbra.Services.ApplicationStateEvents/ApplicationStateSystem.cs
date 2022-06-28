#nullable enable

using Coimbra.Services.Events;
using UnityEngine;

namespace Coimbra.Services.ApplicationStateEvents
{
    /// <summary>
    /// Default implementation for <see cref="IApplicationStateService"/>.
    /// </summary>
    [AddComponentMenu("")]
    [PreloadService]
    public sealed class ApplicationStateSystem : Actor, IApplicationStateService
    {
        private ApplicationStateSystem() { }

        /// <inheritdoc/>
        public bool IsFocused { get; private set; }

        /// <inheritdoc/>
        public bool IsPaused { get; private set; }

        /// <inheritdoc/>
        public void RemoveAllListeners<T>()
            where T : IApplicationStateEvent
        {
            ServiceLocator.GetChecked<IEventService>().RemoveAllListeners<T>();
        }

        /// <inheritdoc/>
        protected override void OnInitialize()
        {
            base.OnInitialize();
            DontDestroyOnLoad(GameObject);
            OnDestroying += HandleDestroying;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            IsFocused = hasFocus;

            new ApplicationFocusEvent(hasFocus).Invoke(this);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            IsPaused = pauseStatus;

            new ApplicationPauseEvent(pauseStatus).Invoke(this);
        }

        private void HandleDestroying(Actor sender, DestroyReason reason)
        {
            if (reason == DestroyReason.ApplicationQuit)
            {
                default(ApplicationQuitEvent).Invoke(this);
            }
        }
    }
}
