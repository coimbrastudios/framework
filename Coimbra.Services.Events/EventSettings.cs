using UnityEngine;

namespace Coimbra.Services.Events
{
    /// <summary>
    /// <see cref="ScriptableSettingsType.RuntimeProjectSettings"/> for <see cref="EventSystem"/>.
    /// </summary>
    /// <seealso cref="IEvent"/>
    /// <seealso cref="IEventService"/>
    /// <seealso cref="EventSystem"/>
    [ProjectSettings(CoimbraUtility.ProjectSettingsPath)]
    [ScriptableSettingsProvider(typeof(FindAnywhereScriptableSettingsProvider))]
    public sealed class EventSettings : ScriptableSettings
    {
        [SerializeField]
        [Tooltip("A warning should be logged when attempting to invoke an event from one of its listeners?")]
        private bool _logRecursiveInvocationWarning = true;

        [SerializeField]
        [Tooltip("Each invocation target should be validated before actually invoking it? Invalid invocation targets will get removed automatically if this is true.")]
        private bool _validateInvocationTargets = true;

        /// <summary>
        /// Gets a value indicating whether a warning should be logged when attempting to invoke an event from one of its listeners.
        /// </summary>
        public bool LogRecursiveInvocationWarning => _logRecursiveInvocationWarning;

        /// <summary>
        /// Gets a value indicating whether each invocation target should be validated before actually invoking it. Invalid invocation targets will get removed automatically if this is true.
        /// </summary>
        public bool ValidateInvocationTargets => _validateInvocationTargets;
    }
}
