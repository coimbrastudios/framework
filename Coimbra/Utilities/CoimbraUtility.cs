using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// General utilities.
    /// </summary>
    public static class CoimbraUtility
    {
        internal const string EditorUserPreferencesPath = UserPreferencesPath + "/Editor";

        internal const string GeneralMenuPath = "Coimbra Framework/";

        internal const string PackageName = "com.coimbrastudios.core";

        internal const string PreferencesMenuPath = ToolsMenuPath + "Preferences/";

        internal const string ProjectAssetsFolderPath = "Assets/" + PackageName;

        internal const string ProjectSettingsFilePath = "ProjectSettings/Packages/" + PackageName;

        internal const string ProjectSettingsPath = "Project/Coimbra Framework";

        internal const string ProjectUserPreferencesPath = UserPreferencesPath + "/Project";

        internal const string ToolsMenuPath = "Tools/Coimbra Framework/";

        internal const string UserPreferencesPath = "Preferences/Coimbra Framework";

        internal const string WindowMenuPath = "Window/Coimbra Framework/";

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
    }
}
