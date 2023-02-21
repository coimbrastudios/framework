using UnityEngine;
using UnityEngine.Serialization;

namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// <see cref="ScriptableSettingsType.RuntimeProjectSettings"/> for <see cref="PlayerLoopSystem"/>.
    /// </summary>
    /// <seealso cref="IPlayerLoopEvent"/>
    /// <seealso cref="IPlayerLoopService"/>
    /// <seealso cref="PlayerLoopSystem"/>
    /// <seealso cref="PlayerLoopEventListener"/>
    /// <seealso cref="PlayerLoopInjectedTimings"/>
    [ProjectSettings(CoimbraUtility.ProjectSettingsPath)]
    public sealed class PlayerLoopSettings : ScriptableSettings
    {
        [SerializeField]
        [FormerlySerializedAs("_defaultTimings")]
        [Tooltip("The default list of events to be fired.")]
        private PlayerLoopInjectedTimings _defaultInjectedTimings = PlayerLoopInjectedTimings.None;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerLoopSettings"/> class.
        /// </summary>
        public PlayerLoopSettings() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerLoopSettings"/> class.
        /// </summary>
        /// <param name="defaultInjectedTimings">The list of events to be fired.</param>
        public PlayerLoopSettings(PlayerLoopInjectedTimings defaultInjectedTimings)
        {
            _defaultInjectedTimings = defaultInjectedTimings;
        }

        /// <summary>
        /// Gets the default list of events to be fired.
        /// </summary>
        public PlayerLoopInjectedTimings DefaultInjectedTimings => _defaultInjectedTimings;
    }
}
