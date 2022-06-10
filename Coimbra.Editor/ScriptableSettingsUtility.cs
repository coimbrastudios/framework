#nullable enable

using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Coimbra.Editor
{
    /// <summary>
    /// Utility methods for <see cref="ScriptableSettings"/>.
    /// </summary>
    public static class ScriptableSettingsUtility
    {
        private const string EditorPrefsFormat = "Coimbra.Editor.ScriptableSettingsUtility.{0}";

        /// <summary>
        /// Create or load a <see cref="ScriptableSettings"/>.
        /// </summary>
        public static ScriptableSettings? LoadOrCreate(Type type, ScriptableSettings.FindHandler? findCallback = null)
        {
            if (!TryGetAttributeData(type, out SettingsScope? settingsScope, out _, out string? filePath, out _))
            {
                return null;
            }

            ScriptableSettingsType filter = ScriptableSettings.GetType(type);

            if (ScriptableSettings.TryGetOrFind(type, out ScriptableSettings value, findCallback) && value.Type == filter)
            {
                return value;
            }

            if (!string.IsNullOrEmpty(filePath))
            {
                Object[] objects = InternalEditorUtility.LoadSerializedFileAndForget(filePath);

                foreach (Object o in objects)
                {
                    if (o is ScriptableSettings match && match.Type == filter)
                    {
                        ScriptableSettings.Set(type, match);

                        return match;
                    }
                }

                value = (ScriptableSettings)ScriptableObject.CreateInstance(type);
            }
            else if (settingsScope == SettingsScope.User)
            {
                value = (ScriptableSettings)ScriptableObject.CreateInstance(type);

                string defaultValue = EditorJsonUtility.ToJson(value, false);
                string newValue = EditorPrefs.GetString(string.Format(EditorPrefsFormat, type.FullName), defaultValue);
                EditorJsonUtility.FromJsonOverwrite(newValue, value);

                if (value.Type != filter)
                {
                    value = (ScriptableSettings)ScriptableObject.CreateInstance(type);
                }
            }

            ScriptableSettings.SetOrOverwrite(type, value);

            return value;
        }

        /// <summary>
        /// Save a <see cref="ScriptableSettings"/>.
        /// </summary>
        public static void Save(this ScriptableSettings scriptableSettings)
        {
            Type type = scriptableSettings.GetType();

            if (!TryGetAttributeData(type, out SettingsScope? settingsScope, out _, out string? filePath, out _))
            {
                return;
            }

            if (filePath == null)
            {
                if (settingsScope == SettingsScope.Project)
                {
                    EditorUtility.SetDirty(scriptableSettings);
                    AssetDatabase.SaveAssetIfDirty(scriptableSettings);

                    return;
                }

                string value = EditorJsonUtility.ToJson(scriptableSettings, true);
                EditorPrefs.SetString(string.Format(EditorPrefsFormat, type.FullName), value);

                return;
            }

            string? directoryName = Path.GetDirectoryName(filePath);

            if (!string.IsNullOrWhiteSpace(directoryName) && !Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            InternalEditorUtility.SaveToSerializedFileAndForget(new Object[]
            {
                scriptableSettings
            }, filePath, true);
        }

        /// <summary>
        /// Gets the file path for a given <see cref="ScriptableSettings"/> type.
        /// </summary>
        public static bool TryGetAttributeData(Type type, out SettingsScope? settingsScope, out string? windowPath, out string? filePath, out string[]? keywords)
        {
            settingsScope = null;
            windowPath = null;
            filePath = null;
            keywords = null;

            ProjectSettingsAttribute projectSettingsAttribute = type.GetCustomAttribute<ProjectSettingsAttribute>();

            if (projectSettingsAttribute != null)
            {
                settingsScope = SettingsScope.Project;
                windowPath = $"{projectSettingsAttribute.WindowPath}/{projectSettingsAttribute.NameOverride ?? CoimbraEditorGUIUtility.ToDisplayName(type.Name)}";
                filePath = projectSettingsAttribute.IsEditorOnly ? $"{projectSettingsAttribute.FileDirectory}/{(projectSettingsAttribute.FileNameOverride ?? $"{type.Name}.asset")}" : null;
                keywords = projectSettingsAttribute.Keywords;
            }

            PreferencesAttribute preferencesAttribute = type.GetCustomAttribute<PreferencesAttribute>();

            if (preferencesAttribute != null)
            {
                settingsScope = SettingsScope.User;
                windowPath = $"{preferencesAttribute.WindowPath}/{preferencesAttribute.NameOverride ?? CoimbraEditorGUIUtility.ToDisplayName(type.Name)}";
                filePath = preferencesAttribute.UseEditorPrefs ? null : $"{preferencesAttribute.FileDirectory}/{(preferencesAttribute.FileNameOverride ?? $"{type.Name}.asset")}";
                keywords = preferencesAttribute.Keywords;
            }

            return windowPath != null;
        }

        /// <summary>
        /// Tries to create or load a <see cref="ScriptableSettings"/>.
        /// </summary>
        public static bool TryLoadOrCreate<T>(out T? scriptableSettings, ScriptableSettings.FindHandler? findCallback = null)
            where T : ScriptableSettings
        {
            scriptableSettings = LoadOrCreate(typeof(T), findCallback) as T;

            return scriptableSettings != null;
        }
    }
}
