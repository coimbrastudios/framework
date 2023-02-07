using UnityEngine;

namespace Coimbra.Services.Events
{
    /// <summary>
    /// <see cref="ScriptableSettingsType.RuntimeProjectSettings"/> for <see cref="EventSystem"/>.
    /// </summary>
    [ProjectSettings(CoimbraUtility.ProjectSettingsPath)]
    public sealed class EventSettings : ScriptableSettings
    {
        [SerializeField]
        [Tooltip("An error message should be logged when attempting to invoke an event from one of its listeners?")]
        private bool _logRecursiveInvocationError = true;

        [SerializeField]
        [Tooltip("Each invocation target should be validated before actually invoking it? Invalid invocation targets will get removed automatically if this is true.")]
        private bool _validateInvocationTargets = true;

        /// <summary>
        /// Gets a value indicating whether an error message should be logged when attempting to invoke an event from one of its listeners.
        /// </summary>
        public bool LogRecursiveInvocationError => _logRecursiveInvocationError;

        /// <summary>
        /// Gets a value indicating whether each invocation target should be validated before actually invoking it. Invalid invocation targets will get removed automatically if this is true.
        /// </summary>
        public bool ValidateInvocationTargets => _validateInvocationTargets;
    }
}
