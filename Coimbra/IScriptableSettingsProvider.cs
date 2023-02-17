#nullable enable

using System;

namespace Coimbra
{
    /// <summary>
    /// Implement this interface to determine how a <see cref="ScriptableSettings"/> should be provided.
    /// </summary>
    /// <seealso cref="ScriptableSettings"/>
    /// <seealso cref="ScriptableSettingsProviderAttribute"/>
    /// <seealso cref="FindAnywhereScriptableSettingsProvider"/>
    /// <seealso cref="LoadOrCreateScriptableSettingsProvider"/>
    public interface IScriptableSettingsProvider
    {
        /// <summary>
        /// Implement this method to provide a <see cref="ScriptableSettings"/> according to its <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="ScriptableSettings"/> type to be provided.</param>
        /// <returns>The <see cref="ScriptableSettings"/> according to the provided <paramref name="type"/>.</returns>
        ScriptableSettings? GetScriptableSettings(Type type);
    }
}
