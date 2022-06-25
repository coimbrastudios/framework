using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// General <see cref="ScriptableSettingsType.EditorProjectSettings"/> settings.
    /// </summary>
    [ProjectSettings(CoimbraUtility.ProjectSettingsPath, "Editor Settings", true)]
    public sealed class CoimbraEditorProjectSettings : ScriptableSettings
    {
        /// <summary>
        /// If true, local analyzers will be disabled for the generated CS project.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("If true, local analyzers will be disabled for the generated CS project")]
        public bool DisableLocalAnalyzers { get; set; } = false;
    }
}
