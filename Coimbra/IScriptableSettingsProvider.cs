#nullable enable

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Implement this interface to determine how a <see cref="ScriptableSettings"/> should be provided.
    /// </summary>
    /// <remarks>
    /// <see cref="InitializeProvider"/> is optional to be implement and by default it does nothing.
    /// <para></para>
    /// <see cref="GetDefaultSettings"/> is also optional, but contains a default implementation that creates the default instance in a lazy way and caches the instance to be used between calls.
    /// </remarks>
    /// <seealso cref="ScriptableSettings"/>
    /// <seealso cref="ScriptableSettingsProviderAttribute"/>
    /// <seealso cref="FindAnywhereScriptableSettingsProvider"/>
    /// <seealso cref="LoadOrCreateScriptableSettingsProvider"/>
    [PublicAPI]
    public interface IScriptableSettingsProvider
    {
        private static readonly Dictionary<Type, ScriptableSettings?> DefaultSettingsMap = new();

        /// <summary>
        /// Implement this to define what should happen when the provider is created for the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="ScriptableSettings"/> type to be provided.</param>
        void InitializeProvider(Type type) { }

        /// <summary>
        /// Implement this to provide a new instance to be set when the <see cref="ScriptableSettings.Get"/> is called but no instance was set already.
        /// </summary>
        /// <param name="type">The <see cref="ScriptableSettings"/> type to be provided.</param>
        /// <returns>The <see cref="ScriptableSettings"/> according to the provided <paramref name="type"/>.</returns>
        ScriptableSettings? GetCurrentSettings(Type type);

        /// <summary>
        /// Implement this to provide a default instance when the <see cref="ScriptableSettings.Get"/> is called with `allowDefault` set to true.
        /// Will only be evaluated if <see cref="GetCurrentSettings"/> returns an invalid value.
        /// </summary>
        /// <param name="type">The <see cref="ScriptableSettings"/> type to be provided.</param>
        /// <returns>The <see cref="ScriptableSettings"/> according to the provided <paramref name="type"/>.</returns>
        ScriptableSettings GetDefaultSettings(Type type)
        {
            if (ApplicationUtility.IsPlayMode && DefaultSettingsMap.TryGetValue(type, out ScriptableSettings? defaultSettings) && defaultSettings != null)
            {
                return defaultSettings;
            }

            defaultSettings = (ScriptableSettings)ScriptableObject.CreateInstance(type);
            DefaultSettingsMap[type] = defaultSettings;

            return defaultSettings;
        }

        internal static void ResetDefaultSettingsMap()
        {
            foreach (KeyValuePair<Type, ScriptableSettings?> pair in DefaultSettingsMap)
            {
                pair.Value.Dispose(true);
            }

            DefaultSettingsMap.Clear();
        }
    }
}
