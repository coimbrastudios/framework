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
                    if (dictionary.ContainsKey(type) || !typeof(ScriptableSettings).IsAssignableFrom(type) || type is not { IsGenericType: false, IsAbstract: false })
                    {
                        continue;
                    }

                    if (ScriptableSettingsProvider.TryCreatePreferencesProvider(type, out ScriptableSettingsProvider? provider))
                    {
                        dictionary.Add(type, provider);
                    }
                    else
                    {
                        ScriptableSettings.Get(type);
                    }
                }

                return dictionary.Values.ToArray();
            }
        }
    }
}
