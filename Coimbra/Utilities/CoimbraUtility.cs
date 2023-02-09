namespace Coimbra
{
    /// <summary>
    /// General utilities.
    /// </summary>
    public static class CoimbraUtility
    {
        /// <summary>
        /// Default menu item path for the game object menu.
        /// </summary>
        public const string GameObjectMenuName = "GameObject/" + GeneralMenuPath;

        /// <summary>
        /// Default menu item name.
        /// </summary>
        public const string GeneralMenuName = "Coimbra Framework";

        /// <summary>
        /// Default menu item path.
        /// </summary>
        public const string GeneralMenuPath = GeneralMenuName + "/";

        /// <summary>
        /// The package name.
        /// </summary>
        public const string PackageName = "com.coimbrastudios.core";

        /// <summary>
        /// Default project settings file path for any <see cref="ScriptableSettingsType.EditorProjectSettings"/>.
        /// </summary>
        public const string ProjectSettingsFilePath = "ProjectSettings/Packages/" + PackageName;

        /// <summary>
        /// Default project settings menu path for any <see cref="ScriptableSettingsType.EditorProjectSettings"/> or <see cref="ScriptableSettingsType.RuntimeProjectSettings"/>.
        /// </summary>
        public const string ProjectSettingsPath = "Project/" + GeneralMenuName;

        /// <summary>
        /// Default menu item path for any editor utility method.
        /// </summary>
        public const string ToolsMenuPath = "Tools/" + GeneralMenuPath;

        /// <summary>
        /// Default project settings file path for any <see cref="ScriptableSettingsType.ProjectUserPreferences"/>.
        /// </summary>
        public const string UserPreferencesFilePath = "UserSettings/Packages/" + PackageName;

        /// <summary>
        /// Default preferences menu path for any <see cref="ScriptableSettingsType.EditorUserPreferences"/> or <see cref="ScriptableSettingsType.ProjectUserPreferences"/>.
        /// </summary>
        public const string UserPreferencesPath = "Preferences/" + GeneralMenuName;

        /// <summary>
        /// Default menu item path for any editor window.
        /// </summary>
        public const string WindowMenuPath = "Window/" + GeneralMenuPath;
    }
}
