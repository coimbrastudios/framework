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
                    if (!dictionary.ContainsKey(type) && typeof(ScriptableSettings).IsAssignableFrom(type) && type is { IsGenericType: false, IsAbstract: false })
                    {
                        dictionary.Add(type, ScriptableSettingsProvider.CreateProjectSettingsProvider(type));
                    }
                }

                return dictionary.Values.ToArray();
            }
        }
    }
}
