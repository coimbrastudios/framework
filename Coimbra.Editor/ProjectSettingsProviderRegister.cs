#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
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
                    if (!dictionary.ContainsKey(type) && typeof(ScriptableSettings).IsAssignableFrom(type) && !type.IsGenericType && !type.IsAbstract)
                    {
                        dictionary.Add(type, ScriptableSettingsProvider.Create(type));
                    }
                }

                return dictionary.Values.ToArray();
            }
        }
    }
}
