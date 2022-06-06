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
                    if (dictionary.ContainsKey(type) || !typeof(ScriptableSettings).IsAssignableFrom(type) || type.IsGenericType || type.IsAbstract)
                    {
                        continue;
                    }

                    if (ScriptableSettingsUtility.TryGetAttributeData(type, out string windowPath, out string filePath, out string[] keywords))
                    {
                        dictionary.Add(type, ScriptableSettingsProvider.CreateProjectSettingsProvider(windowPath, type, filePath, keywords));
                    }
                }

                return dictionary.Values.ToArray();
            }
        }
    }
}
