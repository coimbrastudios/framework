using Coimbra.Services.Events;
using UnityEngine;

namespace Coimbra.Services.ApplicationStateEvents
{
    /// <summary>
    /// Default implementation for <see cref="IApplicationStateService"/>.
    /// </summary>
    [AddComponentMenu("")]
    public sealed class ApplicationStateSystem : EventServiceActorBase<ApplicationStateSystem, IApplicationStateService>, IApplicationStateService
    {
        private readonly EventKey _eventKey = new(EventKey.RestrictionOptions.DisallowInvoke);

        private ApplicationStateSystem() { }

        /// <inheritdoc/>
        public bool IsFocused { get; private set; }

        /// <inheritdoc/>
        public bool IsPaused { get; private set; }

        /// <summary>
        /// Create a new <see cref="IApplicationStateService"/>.
        /// </summary>
        public static IApplicationStateService Create()
        {
            return new GameObject(nameof(ApplicationStateSystem)).AddComponent<ApplicationStateSystem>();
        }

        /// <inheritdoc/>
        protected override void OnEventServiceChanged(IEventService previous, IEventService current)
        {
            previous?.ResetAllEventKeys(_eventKey);

            if (current == null)
            {
                return;
            }

            current.SetEventKey<ApplicationFocusEvent>(_eventKey);
            current.SetEventKey<ApplicationPauseEvent>(_eventKey);
            current.SetEventKey<ApplicationQuitEvent>(_eventKey);
        }

        /// <inheritdoc/>
        protected override void OnInitialize()
        {
            base.OnInitialize();
            DontDestroyOnLoad(CachedGameObject);
            OnDestroyed += HandleDestroyed;
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
            new ApplicationFocusEvent(hasFocus).InvokeAt(EventService, this, _eventKey);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            IsPaused = pauseStatus;
            new ApplicationPauseEvent(pauseStatus).InvokeAt(EventService, this, _eventKey);
        }

        private void HandleDestroyed(Actor sender, DestroyReason reason)
        {
            if (reason == DestroyReason.ApplicationQuit)
            {
                new ApplicationQuitEvent().InvokeAt(EventService, this, _eventKey);
            }
        }
    }
}
