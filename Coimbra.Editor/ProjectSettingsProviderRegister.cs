#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

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
                    if (!dictionary.ContainsKey(type) && typeof(ScriptableSettings).IsAssignableFrom(type) && !type.IsGenericType && !type.IsAbstract)
                    {
                        bool hasProvider = ScriptableSettingsProvider.TryCreate(type, out ScriptableSettingsProvider? provider);
                        Debug.Assert(hasProvider, $"{type} should not have null as the {nameof(ProjectSettingsAttribute.WindowPath)}.");
                        dictionary.Add(type, provider!);
                    }
                }

                return dictionary.Values.ToArray();
            }
        }
    }
}
