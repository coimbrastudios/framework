#nullable enable

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Coimbra
{
    /// <summary>
    /// Finds with <see cref="ObjectUtility.FindAllAnywhere"/>, returning null if none. Also logs a warning if more than 1 is found.
    /// </summary>
    /// <seealso cref="ScriptableSettings"/>
    /// <seealso cref="ScriptableSettingsProviderAttribute"/>
    /// <seealso cref="IScriptableSettingsProvider"/>
    /// <seealso cref="LoadOrCreateScriptableSettingsProvider"/>
    public sealed class FindAnywhereScriptableSettingsProvider : IScriptableSettingsProvider
    {
        /// <summary>
        /// Default instance that can be safely reused.
        /// </summary>
        public static readonly FindAnywhereScriptableSettingsProvider Default = new();

        /// <inheritdoc/>
        public ScriptableSettings? GetCurrentSettings(Type type)
        {
            Object[] rawValues = ObjectUtility.FindAllAnywhere(type);

            if (rawValues.Length == 0)
            {
                return null;
            }

            if (rawValues.Length > 1)
            {
                Debug.LogWarning($"It was expected a single loaded object of type {type}, but it was found {rawValues.Length}!");
#if UNITY_EDITOR
                foreach (Object rawValue in rawValues)
                {
                    Debug.Log(UnityEditor.AssetDatabase.GetAssetPath(rawValue), rawValue);
                }
#endif
            }

            ScriptableSettings result = (ScriptableSettings)rawValues[0];

            return result;
        }
    }
}
