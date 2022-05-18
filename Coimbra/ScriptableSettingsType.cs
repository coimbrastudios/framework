namespace Coimbra
{
    /// <summary>
    /// The type of <see cref="ScriptableSettings"/> based on the presence of either <see cref="PreferencesAttribute"/> or <see cref="ProjectSettingsAttribute"/>.
    /// </summary>
    public enum ScriptableSettingsType
    {
        /// <summary>
        /// Neither <see cref="PreferencesAttribute"/> or <see cref="ProjectSettingsAttribute"/> is defined.
        /// </summary>
        Custom,

        /// <summary>
        /// <see cref="ProjectSettingsAttribute"/> is defined and <see cref="ProjectSettingsAttribute.IsEditorOnly"/> is true.
        /// </summary>
        EditorProjectSettings,

        /// <summary>
        /// <see cref="ProjectSettingsAttribute"/> is defined and <see cref="ProjectSettingsAttribute.IsEditorOnly"/> is false.
        /// </summary>
        RuntimeProjectSettings,

        /// <summary>
        /// <see cref="PreferencesAttribute"/> is defined and <see cref="PreferencesAttribute.UseEditorPrefs"/> is true.
        /// </summary>
        EditorUserPreferences,

        /// <summary>
        /// <see cref="PreferencesAttribute"/> is defined and <see cref="PreferencesAttribute.UseEditorPrefs"/> is false.
        /// </summary>
        ProjectUserPreferences,
    }
}
