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

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerLoopSettings"/> class.
        /// </summary>
        public PlayerLoopSettings() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerLoopSettings"/> class.
        /// </summary>
        /// <param name="defaultTimings">The list of events to be fired.</param>
        public PlayerLoopSettings(PlayerLoopTimingEvents defaultTimings)
        {
            _defaultTimings = defaultTimings;
        }

        /// <summary>
        /// Gets the default list of events to be fired.
        /// </summary>
        public PlayerLoopTimingEvents DefaultTimings => _defaultTimings;
    }
}
