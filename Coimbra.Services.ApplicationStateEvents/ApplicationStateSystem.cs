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
    public sealed class ApplicationStateSystem : ServiceActorBase<ApplicationStateSystem, IApplicationStateService>, IApplicationStateService
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
            OwningLocator?.Get<IEventService>()?.RemoveAllListeners<T>();
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

            if (OwningLocator != null)
            {
                new ApplicationFocusEvent(hasFocus).Invoke(OwningLocator, this);
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            IsPaused = pauseStatus;

            if (OwningLocator != null)
            {
                new ApplicationPauseEvent(pauseStatus).Invoke(OwningLocator, this);
            }
        }

        private void HandleDestroying(Actor sender, DestroyReason reason)
        {
            if (reason == DestroyReason.ApplicationQuit && OwningLocator != null)
            {
                new ApplicationQuitEvent().Invoke(OwningLocator, this);
            }
        }
    }
}
