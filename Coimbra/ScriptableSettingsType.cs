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
        Custom = 0,

        /// <summary>
        /// <see cref="ProjectSettingsAttribute"/> is defined and <see cref="ProjectSettingsAttribute.IsEditorOnly"/> is true.
        /// </summary>
        EditorProjectSettings = 1,

        /// <summary>
        /// <see cref="ProjectSettingsAttribute"/> is defined and <see cref="ProjectSettingsAttribute.IsEditorOnly"/> is false.
        /// </summary>
        RuntimeProjectSettings = 2,

        /// <summary>
        /// <see cref="PreferencesAttribute"/> is defined and <see cref="PreferencesAttribute.UseEditorPrefs"/> is true.
        /// </summary>
        EditorUserPreferences = 3,

        /// <summary>
        /// <see cref="PreferencesAttribute"/> is defined and <see cref="PreferencesAttribute.UseEditorPrefs"/> is false.
        /// </summary>
        ProjectUserPreferences = 4,
    }
}
