using System;
using System.IO;
using UnityEditor;

namespace Coimbra.Editor
{
    /// <summary>
    ///     General editor utilities.
    /// </summary>
    public static class CSEditorUtility
    {
        internal const string PackageName = "com.coimbrastudios.framework";
        internal const string UserPreferencesPath = "Preferences/CS Framework";
        internal const string ProjectSettingsPath = "Project/CS Framework";

        /// <summary>
        /// Is currently on edit mode?
        /// </summary>
        public static bool IsEditMode => !EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode;

        /// <summary>
        /// Is currently on play mode?
        /// </summary>
        public static bool IsPlayMode => EditorApplication.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode;

        internal static void EnsureDirectoryExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        internal static bool TryMatchSearch(string searchContext, string target)
        {
            if (searchContext == null)
            {
                return true;
            }

            searchContext = searchContext.Trim();

            if (string.IsNullOrEmpty(searchContext))
            {
                return true;
            }

            string[] split = searchContext.Split(' ');

            foreach (string value in split)
            {
                if (!string.IsNullOrEmpty(value) && target.IndexOf(value, StringComparison.InvariantCultureIgnoreCase) > -1)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
