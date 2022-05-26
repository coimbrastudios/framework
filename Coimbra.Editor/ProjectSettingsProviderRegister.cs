#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace Coimbra.Editor
{
    internal static class ProjectSettingsProviderRegister
    {
        [SettingsProviderGroup]
        private static SettingsProvider[] CreateProjectSettingsProviders()
        {
            using (DictionaryPool.Pop(out Dictionary<Type, SettingsProvider> dictionary))
            {
                foreach (Type type in TypeCache.GetTypesWithAttribute<ProjectSettingsAttribute>())
                {
                    if (dictionary.ContainsKey(type) || !typeof(ScriptableSettings).IsAssignableFrom(type) || type.IsGenericType || type.IsAbstract)
                    {
                        continue;
                    }

                    ProjectSettingsAttribute attribute = type.GetCustomAttribute<ProjectSettingsAttribute>();
                    string path = $"{attribute.WindowPath}/{attribute.NameOverride ?? CoimbraEditorGUIUtility.ToDisplayName(type.Name)}";

                    if (attribute.IsEditorOnly)
                    {
                        dictionary.Add(type, ScriptableSettingsProvider.CreateProjectSettingsProvider(path, type, $"{attribute.FileDirectory}/{(attribute.FileNameOverride ?? $"{type.Name}.asset")}", attribute.Keywords));
                    }
                    else
                    {
                        dictionary.Add(type, ScriptableSettingsProvider.CreateProjectSettingsProvider(path, type, null, attribute.Keywords));
                    }
                }

                return dictionary.Values.ToArray();
            }
        }
    }
}
