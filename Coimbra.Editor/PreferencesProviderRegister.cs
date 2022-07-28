#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

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
                    if (!dictionary.ContainsKey(type) && typeof(ScriptableSettings).IsAssignableFrom(type) && !type.IsGenericType && !type.IsAbstract)
                    {
                        if (ScriptableSettingsProvider.TryCreate(type, out ScriptableSettingsProvider? provider))
                        {
                            dictionary.Add(type, provider);
                        }
                        else
                        {
                            ScriptableSettingsUtility.LoadOrCreate(type);
                        }
                    }
                }

                return dictionary.Values.ToArray();
            }
        }
    }
}
