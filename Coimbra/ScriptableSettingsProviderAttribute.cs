#nullable enable

using JetBrains.Annotations;
using System;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Add this to a <see cref="ScriptableObject"/> to set the <see cref="IScriptableSettingsProvider"/> to be used for the given type and its children.
    /// </summary>
    /// <seealso cref="ScriptableSettings"/>
    /// <seealso cref="IScriptableSettingsProvider"/>
    /// <seealso cref="FindAnywhereScriptableSettingsProvider"/>
    /// <seealso cref="LoadOrCreateScriptableSettingsProvider"/>
    [AttributeUsage(AttributeTargets.Class)]
    [BaseTypeRequired(typeof(ScriptableObject))]
    public sealed class ScriptableSettingsProviderAttribute : Attribute
    {
        /// <summary>
        /// The <see cref="IScriptableSettingsProvider"/> to be used.
        /// </summary>
        public readonly Type Type;

        public ScriptableSettingsProviderAttribute(Type type)
        {
            Debug.Assert(type.CanCreateInstance());
            Debug.Assert(typeof(IScriptableSettingsProvider).IsAssignableFrom(type));

            Type = type;
        }
    }
}
