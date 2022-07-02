#if !UNITY_2021_3_OR_NEWER
using UnityEditor;
#endif

namespace Coimbra.Editor
{
    /// <summary>
    /// General <see cref="ScriptableSettingsType.EditorUserPreferences"/> settings.
    /// </summary>
#if !UNITY_2021_3_OR_NEWER
    [InitializeOnLoad]
    [Preferences(CoimbraUtility.UserPreferencesPath, "Editor Settings", true)]
#endif
    public sealed class CoimbraEditorUserSettings : ScriptableSettings
    {
#if !UNITY_2021_3_OR_NEWER
        static CoimbraEditorUserSettings()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= HandleBeforeAssemblyReload;
            AssemblyReloadEvents.beforeAssemblyReload += HandleBeforeAssemblyReload;
        }
#endif

#if UNITY_2021_3_OR_NEWER
        [System.Obsolete("Unity has now a built-in functionality for it in its Console window. This property will have no effect.")]
#else
        /// <summary>
        /// Gets or sets a value indicating whether the console will be cleared upon a script reload.
        /// </summary>
        [JetBrains.Annotations.PublicAPI]
        [field: UnityEngine.SerializeField]
        [field: UnityEngine.Tooltip("If true, console will be cleared upon a script reload.")]
#endif
        public bool ClearConsoleOnReload { get; set; }

#if !UNITY_2021_3_OR_NEWER
        private static void HandleBeforeAssemblyReload()
        {
            ScriptableSettingsUtility.TryLoadOrCreate(out CoimbraEditorUserSettings settings, FindSingle);
            UnityEngine.Debug.Assert(settings);

            if (settings.ClearConsoleOnReload)
            {
                CoimbraEditorUtility.ClearConsoleWindow();
            }
        }
#endif
    }
}
