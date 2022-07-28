#nullable enable

using CoimbraInternal.Editor;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    [InitializeOnLoad]
    public static class ScriptableSettingsUtility
    {
        private static readonly Object?[] SaveTarget = new Object?[1];

        static ScriptableSettingsUtility()
        {
            UnityEditorInternals.OnEditorApplicationFocusChanged -= HandleEditorApplicationFocusChanged;
            UnityEditorInternals.OnEditorApplicationFocusChanged += HandleEditorApplicationFocusChanged;
        }

        /// <summary>
        /// Reloads a <see cref="ScriptableSettings"/>.
        /// </summary>
        public static void Reload(this ScriptableSettings scriptableSettings)
        {
            Type type = scriptableSettings.GetType();

            if (!TryGetAttributeData(type, out SettingsScope? settingsScope, out _, out string? filePath, out _))
            {
                return;
            }

            if (!string.IsNullOrEmpty(filePath))
            {
                if (!TryLoad(type, filePath, scriptableSettings.Type, out ScriptableSettings? target))
                {
                    target = (ScriptableSettings)ScriptableObject.CreateInstance(type);
                }

                JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(target), scriptableSettings);
                Object.DestroyImmediate(target, false);
            }
            else if (settingsScope == SettingsScope.User)
            {
                ScriptableSettings target = (ScriptableSettings)ScriptableObject.CreateInstance(type);
                string defaultValue = EditorJsonUtility.ToJson(target, false);
                string newValue = EditorPrefs.GetString(GetPrefsKey(type), defaultValue);
                EditorJsonUtility.FromJsonOverwrite(newValue, scriptableSettings);

                if (scriptableSettings.Type != ScriptableSettings.GetType(type))
                {
                    JsonUtility.FromJsonOverwrite(defaultValue, scriptableSettings);
                }

                Object.DestroyImmediate(target, false);
            }
        }

        /// <inheritdoc cref="LoadOrCreate"/>
        public static T? LoadOrCreate<T>(ScriptableSettings.FindHandler? findCallback = null)
            where T : ScriptableSettings
        {
            return (T?)LoadOrCreate(typeof(T), findCallback);
        }

        /// <summary>
        /// Create or load a <see cref="ScriptableSettings"/>.
        /// </summary>
        public static ScriptableSettings? LoadOrCreate(Type type, ScriptableSettings.FindHandler? findCallback = null)
        {
            ScriptableSettingsType filter = ScriptableSettings.GetType(type);

            if (ScriptableSettings.TryGetOrFind(type, out ScriptableSettings value, findCallback))
            {
                if (value.Type == filter)
                {
                    return value;
                }

                Debug.LogWarning($"Destroying {value} because its type changed from {value.Type} to {filter}!", value);
                Object.DestroyImmediate(value, true);
            }

            if (!TryGetAttributeData(type, out SettingsScope? settingsScope, out _, out string? filePath, out _))
            {
                return null;
            }

            if (!string.IsNullOrEmpty(filePath))
            {
                if (TryLoad(type, filePath, filter, out ScriptableSettings? scriptableSettings))
                {
                    return scriptableSettings;
                }

                value = (ScriptableSettings)ScriptableObject.CreateInstance(type);
            }
            else if (settingsScope == SettingsScope.User)
            {
                value = (ScriptableSettings)ScriptableObject.CreateInstance(type);

                string defaultValue = EditorJsonUtility.ToJson(value, false);
                string newValue = EditorPrefs.GetString(GetPrefsKey(type), defaultValue);
                EditorJsonUtility.FromJsonOverwrite(newValue, value);

                if (value.Type != filter)
                {
                    Object.DestroyImmediate(value, false);

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
                EditorPrefs.SetString(GetPrefsKey(type), value);

                return;
            }

            string? directoryName = Path.GetDirectoryName(filePath);

            if (!string.IsNullOrWhiteSpace(directoryName) && !Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            SaveTarget[0] = scriptableSettings;
            InternalEditorUtility.SaveToSerializedFileAndForget(SaveTarget, filePath, true);

            SaveTarget[0] = null;
        }

        /// <summary>
        /// Gets the file path for a given <see cref="ScriptableSettings"/> type.
        /// </summary>
        public static bool TryGetAttributeData(Type type, [NotNullWhen(true)] out SettingsScope? settingsScope, out string? windowPath, out string? filePath, out string[]? keywords)
        {
            settingsScope = null;
            windowPath = null;
            filePath = null;
            keywords = null;

            ProjectSettingsAttribute projectSettingsAttribute = type.GetCustomAttribute<ProjectSettingsAttribute>();

            if (projectSettingsAttribute != null)
            {
                settingsScope = SettingsScope.Project;
                windowPath = $"{projectSettingsAttribute.WindowPath}/{projectSettingsAttribute.NameOverride ?? CoimbraGUIUtility.GetDisplayName(type.Name)}";
                filePath = projectSettingsAttribute.IsEditorOnly && projectSettingsAttribute.FileDirectory != null ? $"{projectSettingsAttribute.FileDirectory}/{projectSettingsAttribute.FileNameOverride ?? $"{type.Name}.asset"}" : null;
                keywords = projectSettingsAttribute.Keywords;
            }

            PreferencesAttribute preferencesAttribute = type.GetCustomAttribute<PreferencesAttribute>();

            if (preferencesAttribute != null)
            {
                settingsScope = SettingsScope.User;
                windowPath = preferencesAttribute.WindowPath != null ? $"{preferencesAttribute.WindowPath}/{preferencesAttribute.NameOverride ?? CoimbraGUIUtility.GetDisplayName(type.Name)}" : null;
                filePath = preferencesAttribute.UseEditorPrefs ? null : $"{preferencesAttribute.FileDirectory}/{preferencesAttribute.FileNameOverride ?? $"{type.Name}.asset"}";
                keywords = preferencesAttribute.Keywords;
            }

            return settingsScope != null;
        }

        /// <summary>
        /// Tries to create or load a <see cref="ScriptableSettings"/>.
        /// </summary>
        public static bool TryLoadOrCreate<T>([NotNullWhen(true)] out T? scriptableSettings, ScriptableSettings.FindHandler? findCallback = null)
            where T : ScriptableSettings
        {
            scriptableSettings = LoadOrCreate(typeof(T), findCallback) as T;

            return scriptableSettings != null;
        }

        /// <summary>
        /// Gets the prefs key to use with a given type.
        /// </summary>
        public static string GetPrefsKey(Type type)
        {
            return $"{CoimbraUtility.PackageName}.{type.FullName}";
        }

        private static void HandleEditorApplicationFocusChanged(bool isFocused)
        {
            foreach (KeyValuePair<Type, ScriptableSettings> pair in ScriptableSettings.Map)
            {
                pair.Value.Reload();
            }
        }

        private static bool TryLoad(Type type, string? filePath, ScriptableSettingsType filter, [NotNullWhen(true)] out ScriptableSettings? scriptableSettings)
        {
            Object[] objects = InternalEditorUtility.LoadSerializedFileAndForget(filePath);

            foreach (Object o in objects)
            {
                if (o is ScriptableSettings match && match.Type == filter)
                {
                    ScriptableSettings.Set(type, match);
                    scriptableSettings = match;

                    return true;
                }
            }

            scriptableSettings = null;

            return false;
        }
    }
}
