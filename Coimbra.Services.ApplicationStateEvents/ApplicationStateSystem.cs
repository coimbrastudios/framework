#nullable enable

using Coimbra.Services.Events;
using UnityEngine;

namespace Coimbra.Services.ApplicationStateEvents
{
    /// <summary>
    /// Default implementation for <see cref="IApplicationStateService"/>.
    /// </summary>
    /// <remarks>
    /// It uses <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnApplicationFocus.html">OnApplicationFocus</a>, <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnApplicationPause.html">OnApplicationPause</a>, and <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnApplicationQuit.html">OnApplicationQuit</a> to invoke the respective events.
    /// <para></para>
    /// The events are invoked using <see cref="IEventService"/> and this implementation makes use of <see cref="Object.DontDestroyOnLoad"/> to persist between scenes.
    /// </remarks>
    /// <seealso cref="ApplicationFocusEvent"/>
    /// <seealso cref="ApplicationPauseEvent"/>
    /// <seealso cref="ApplicationQuitEvent"/>
    /// <seealso cref="IApplicationStateEvent"/>
    /// <seealso cref="IApplicationStateService"/>
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
