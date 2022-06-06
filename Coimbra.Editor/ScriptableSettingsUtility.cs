#nullable enable

using System;
using System.Reflection;

namespace Coimbra.Editor
{
    /// <summary>
    /// Utility methods for <see cref="ScriptableSettings"/>.
    /// </summary>
    public static class ScriptableSettingsUtility
    {
        /// <summary>
        /// Gets the file path for a given <see cref="ScriptableSettings"/> type.
        /// </summary>
        public static bool TryGetAttributeData(Type type, out string? windowPath, out string? filePath, out string[]? keywords)
        {
            windowPath = null;
            filePath = null;
            keywords = null;

            ProjectSettingsAttribute projectSettingsAttribute = type.GetCustomAttribute<ProjectSettingsAttribute>();

            if (projectSettingsAttribute != null)
            {
                windowPath = $"{projectSettingsAttribute.WindowPath}/{projectSettingsAttribute.NameOverride ?? CoimbraEditorGUIUtility.ToDisplayName(type.Name)}";
                filePath = projectSettingsAttribute.IsEditorOnly ? $"{projectSettingsAttribute.FileDirectory}/{(projectSettingsAttribute.FileNameOverride ?? $"{type.Name}.asset")}" : null;
                keywords = projectSettingsAttribute.Keywords;
            }

            PreferencesAttribute preferencesAttribute = type.GetCustomAttribute<PreferencesAttribute>();

            if (preferencesAttribute != null)
            {
                windowPath = $"{preferencesAttribute.WindowPath}/{preferencesAttribute.NameOverride ?? CoimbraEditorGUIUtility.ToDisplayName(type.Name)}";
                filePath = preferencesAttribute.UseEditorPrefs ? null : $"{preferencesAttribute.FileDirectory}/{(preferencesAttribute.FileNameOverride ?? $"{type.Name}.asset")}";
                keywords = preferencesAttribute.Keywords;
            }

            return windowPath != null;
        }
    }
}
