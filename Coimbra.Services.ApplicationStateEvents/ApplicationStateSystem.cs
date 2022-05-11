using Coimbra.Services.Events;
using UnityEngine;

namespace Coimbra.Services.ApplicationStateEvents
{
    /// <summary>
    /// Default implementation for <see cref="IApplicationStateService"/>.
    /// </summary>
    [AddComponentMenu("")]
    public sealed class ApplicationStateSystem : ServiceActorBase<ApplicationStateSystem, IApplicationStateService>, IApplicationStateService
    {
        private ApplicationStateSystem() { }

        /// <inheritdoc/>
        public bool IsFocused { get; private set; }

        /// <inheritdoc/>
        public bool IsPaused { get; private set; }

        private IEventService EventService => OwningLocator?.Get<IEventService>();

        /// <summary>
        /// Create a new <see cref="IApplicationStateService"/>.
        /// </summary>
        public static IApplicationStateService Create()
        {
            return new GameObject(nameof(ApplicationStateSystem)).AddComponent<ApplicationStateSystem>();
        }

        /// <inheritdoc/>
        protected override void OnDestroyed()
        {
            ApplicationFocusEvent.RemoveAllListenersAt(EventService);
            ApplicationPauseEvent.RemoveAllListenersAt(EventService);
            ApplicationQuitEvent.RemoveAllListenersAt(EventService);
            base.OnDestroyed();
        }

        /// <inheritdoc/>
        protected override void OnInitialize()
        {
            base.OnInitialize();
            DontDestroyOnLoad(CachedGameObject);
            OnDestroying += HandleDestroying;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void HandleSubsystemRegistration()
        {
            ServiceLocator.Shared.SetCreateCallback(Create, false);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void HandleBeforeSceneLoad()
        {
            ServiceLocator.Shared.Get<IApplicationStateService>();
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
