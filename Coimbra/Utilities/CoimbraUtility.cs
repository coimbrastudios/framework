using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// General utilities.
    /// </summary>
    public static class CoimbraUtility
    {
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

        /// <summary>
        /// Returns true if the application is currently in Edit Mode. Always return false in a build.
        /// </summary>
        public static bool IsEditMode
        {
            get
            {
#if UNITY_EDITOR
                return !Application.isPlaying;
#else
                return false;
#endif
            }
        }

        /// <summary>
        /// Returns true if the application is currently in Play Mode. Always return true in a build.
        /// </summary>
        public static bool IsPlayMode
        {
            get
            {
#if UNITY_EDITOR
                return Application.isPlaying;
#else
                return true;
#endif
            }
        }

        /// <summary>
        /// Are scripts currently reloading?
        /// </summary>
        public static bool IsReloadingScripts { get; internal set; }

        /// <summary>
        /// Returns true if the code is running inside Unity Cloud Build.
        /// </summary>
        public static bool IsUnityCloudBuild
        {
            get
            {
#if UNITY_CLOUD_BUILD
                return true;
#else
                return false;
#endif
            }
        }
    }
}
