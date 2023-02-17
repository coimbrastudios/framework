#nullable enable

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Coimbra
{
    /// <summary>
    /// Finds with <see cref="FindAnywhereScriptableSettingsProvider.Default"/>, but fallbacks to loading from the disk. If nothing to load, then a new instance will be returned.
    /// </summary>
    /// <seealso cref="ScriptableSettings"/>
    /// <seealso cref="ScriptableSettingsProviderAttribute"/>
    /// <seealso cref="IScriptableSettingsProvider"/>
    /// <seealso cref="FindAnywhereScriptableSettingsProvider"/>
    public sealed class LoadOrCreateScriptableSettingsProvider : IScriptableSettingsProvider
    {
        /// <summary>
        /// Default instance that can be safely reused.
        /// </summary>
        public static readonly LoadOrCreateScriptableSettingsProvider Default = new();

        /// <inheritdoc/>
        public ScriptableSettings GetScriptableSettings(Type type)
        {
            ScriptableSettingsType filter = ScriptableSettings.GetTypeData(type, out _, out string? filePath, out _);
            ScriptableSettings? value;

            if (!Application.isEditor || filter == ScriptableSettingsType.Custom)
            {
                if (!FindAnywhereScriptableSettingsProvider.Default.GetScriptableSettings(type).TryGetValid(out value))
                {
                    value = (ScriptableSettings)ScriptableObject.CreateInstance(type);
                }

                return value;
            }

#if UNITY_EDITOR
            if (FindAnywhereScriptableSettingsProvider.Default.GetScriptableSettings(type).TryGetValid(out value))
            {
                if (filePath != null)
                {
                    string assetPath = UnityEditor.AssetDatabase.GetAssetPath(value);

                    if (!string.IsNullOrWhiteSpace(assetPath))
                    {
                        ScriptableSettings copy = Object.Instantiate(value);
                        Debug.LogWarning($"Moving {value} from {assetPath} to {filePath}!", copy);
                        Object.DestroyImmediate(value, true);
                        UnityEditor.AssetDatabase.DeleteAsset(assetPath);
                        copy.Save();

                        return copy;
                    }
                }

                if (value.Type == filter)
                {
                    return value;
                }

                Debug.LogWarning($"Destroying {value} because its type changed from {value.Type} to {filter}!", value);
                Object.DestroyImmediate(value, true);
            }

            if (!string.IsNullOrEmpty(filePath))
            {
                if (ScriptableSettings.TryLoad(type, filePath, filter, out ScriptableSettings? scriptableSettings))
                {
                    return scriptableSettings;
                }

                value = (ScriptableSettings)ScriptableObject.CreateInstance(type);
            }
            else if (filter.IsUserPreferences())
            {
                value = (ScriptableSettings)ScriptableObject.CreateInstance(type);

                string defaultValue = UnityEditor.EditorJsonUtility.ToJson(value, false);
                string newValue = UnityEditor.EditorPrefs.GetString(ApplicationUtility.GetPrefsKey(type), defaultValue);
                UnityEditor.EditorJsonUtility.FromJsonOverwrite(newValue, value);

                if (value.Type != filter)
                {
                    Object.DestroyImmediate(value, false);

                    value = (ScriptableSettings)ScriptableObject.CreateInstance(type);
                }
            }
#endif

            return value != null ? value : (ScriptableSettings)ScriptableObject.CreateInstance(type);
        }
    }
}
