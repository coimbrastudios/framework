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

        private IEventService? EventService => OwningLocator?.Get<IEventService>();

        /// <inheritdoc/>
        public void RemoveAllListeners()
        {
            if (EventService == null)
            {
                return;
            }

            ApplicationFocusEvent.RemoveAllListenersAt(EventService);
            ApplicationPauseEvent.RemoveAllListenersAt(EventService);
            ApplicationQuitEvent.RemoveAllListenersAt(EventService);
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
            new ApplicationFocusEvent(hasFocus).TryInvokeAt(EventService, this);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            IsPaused = pauseStatus;
            new ApplicationPauseEvent(pauseStatus).TryInvokeAt(EventService, this);
        }

        private void HandleDestroying(Actor sender, DestroyReason reason)
        {
            if (reason == DestroyReason.ApplicationQuit)
            {
                new ApplicationQuitEvent().TryInvokeAt(EventService, this);
            }
        }
    }
}
