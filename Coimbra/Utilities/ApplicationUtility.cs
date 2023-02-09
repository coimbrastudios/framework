using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// General application utilities.
    /// </summary>
    public static class ApplicationUtility
    {
        /// <summary>
        /// Gets a value indicating whether the application is currently in Edit Mode. Always return false in a build.
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
        /// Gets a value indicating whether the application is currently in its first frame. Always false if <see cref="IsPlayMode"/> is false.
        /// </summary>
        public static bool IsFirstFrame => IsPlayMode && Time.frameCount == 0;

        /// <summary>
        /// Gets a value indicating whether the application is currently in Play Mode. Always return true in a build.
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
        /// Gets a value indicating whether the scripts currently reloading.
        /// </summary>
        public static bool IsReloadingScripts { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the code is running inside Unity Cloud Build.
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
