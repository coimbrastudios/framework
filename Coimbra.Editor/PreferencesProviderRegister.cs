#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    internal static class PreferencesProviderRegister
    {
        [SettingsProviderGroup]
        private static SettingsProvider[] CreatePreferencesProviders()
        {
            using (DictionaryPool.Pop(out Dictionary<Type, SettingsProvider> dictionary))
            {
                foreach (Type type in TypeCache.GetTypesWithAttribute<PreferencesAttribute>())
                {
                    if (dictionary.ContainsKey(type))
                    {
                        continue;
                    }

                    Debug.Assert(!type.IsAbstract, $"{nameof(PreferencesAttribute)} should not be used on abstract type {type.FullName}!");
                    Debug.Assert(!type.IsGenericType, $"{nameof(PreferencesAttribute)} should not be used on generic type {type.FullName}!");
                    Debug.Assert(typeof(ScriptableSettings).IsAssignableFrom(type), $"{type.FullName} should be a {nameof(ScriptableSettings)} to use {nameof(PreferencesAttribute)}!");

                    PreferencesAttribute attribute = type.GetCustomAttribute<PreferencesAttribute>();
                    string path = $"{attribute.WindowPath}/{attribute.NameOverride ?? CoimbraEditorGUIUtility.ToDisplayName(type.Name)}";

                    if (attribute.UseEditorPrefs)
                    {
                        dictionary.Add(type, ScriptableSettingsProvider.CreatePreferencesProvider(path, type, null, attribute.Keywords));
                    }
                    else
                    {
                        dictionary.Add(type, ScriptableSettingsProvider.CreatePreferencesProvider(path, type, $"{attribute.FileDirectory}/{(attribute.FileNameOverride ?? $"{type.Name}.asset")}", attribute.Keywords));
                    }
                }

                return dictionary.Values.ToArray();
            }
        }
    }
}
