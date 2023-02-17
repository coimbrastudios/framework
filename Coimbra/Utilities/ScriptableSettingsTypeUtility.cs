using System.Runtime.CompilerServices;

namespace Coimbra
{
    /// <summary>
    /// Utility methods for <see cref="ScriptableSettingsType"/> type.
    /// </summary>
    /// <seealso cref="ScriptableSettings"/>
    /// <seealso cref="ScriptableSettingsType"/>
    public static class ScriptableSettingsTypeUtility
    {
        /// <summary>
        /// True if the asset will never be included in the build.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEditorOnly(this ScriptableSettingsType type)
        {
            return type != ScriptableSettingsType.Custom && type != ScriptableSettingsType.RuntimeProjectSettings;
        }

        /// <summary>
        /// True if the asset type has <see cref="ProjectSettingsAttribute"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsProjectSettings(this ScriptableSettingsType type)
        {
            return type is ScriptableSettingsType.EditorProjectSettings or ScriptableSettingsType.RuntimeProjectSettings;
        }

        /// <summary>
        /// True if the asset type has <see cref="PreferencesAttribute"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsUserPreferences(this ScriptableSettingsType type)
        {
            return type is ScriptableSettingsType.EditorUserPreferences or ScriptableSettingsType.ProjectUserPreferences;
        }
    }
}
