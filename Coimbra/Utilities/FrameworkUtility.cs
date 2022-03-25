using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Coimbra Studios general utilities.
    /// </summary>
    public static class FrameworkUtility
    {
        internal const string AddComponentMenuPath = "CS Framework/";
        internal const string PackageName = "com.coimbrastudios";
        internal const string ProjectSettingsPath = "Project/CS Framework";
        internal const string UserPreferencesPath = "Preferences/CS Framework";

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
    }
}
