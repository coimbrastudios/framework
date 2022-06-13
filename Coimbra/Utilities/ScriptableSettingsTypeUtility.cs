using System.Runtime.CompilerServices;

namespace Coimbra
{
    /// <summary>
    /// Utility methods for <see cref="ScriptableSettingsType"/> type.
    /// </summary>
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
    }
}
