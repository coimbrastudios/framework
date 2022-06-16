using UnityEngine;

namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// <see cref="ScriptableSettingsType.RuntimeProjectSettings"/> for <see cref="PlayerLoopSystem"/>.
    /// </summary>
    [ProjectSettings(CoimbraUtility.ProjectSettingsPath)]
    public sealed class PlayerLoopSettings : ScriptableSettings
    {
        [SerializeField]
        [Tooltip("The default list of events to be fired.")]
        private PlayerLoopTimingEvents _defaultTimings = PlayerLoopTimingEvents.Standard;

        public PlayerLoopSettings() { }

        /// <param name="defaultTimings">The list of events to be fired.</param>
        public PlayerLoopSettings(PlayerLoopTimingEvents defaultTimings)
        {
            _defaultTimings = defaultTimings;
        }

        /// <summary>
        /// The default list of events to be fired.
        /// </summary>
        public PlayerLoopTimingEvents DefaultTimings => _defaultTimings;
    }
}
