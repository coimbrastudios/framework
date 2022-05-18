#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    internal static class ProjectSettingsProviderRegister
    {
        [SettingsProviderGroup]
        private static SettingsProvider[] CreatePoolingSettingsProvider()
        {
            using (StringBuilderPool.Pop(out StringBuilder pathBuilder))
            {
                using (DictionaryPool.Pop(out Dictionary<Type, SettingsProvider> dictionary))
                {
                    foreach (Type type in TypeCache.GetTypesWithAttribute<ProjectSettingsAttribute>())
                    {
                        if (dictionary.ContainsKey(type))
                        {
                            continue;
                        }

                        Debug.Assert(!type.IsAbstract, $"{nameof(ProjectSettingsAttribute)} should not be used on abstract type {type.FullName}!");
                        Debug.Assert(!type.IsGenericType, $"{nameof(ProjectSettingsAttribute)} should not be used on generic type {type.FullName}!");
                        Debug.Assert(typeof(ScriptableSettings).IsAssignableFrom(type), $"{type.FullName} should be a {nameof(ScriptableSettings)} to use {nameof(ProjectSettingsAttribute)}!");

                        ProjectSettingsAttribute attribute = type.GetCustomAttribute<ProjectSettingsAttribute>();
                        pathBuilder.Append($"{attribute.ProjectSettingsSection}/");

                        if (!string.IsNullOrWhiteSpace(attribute.ProjectSettingsPath))
                        {
                            pathBuilder.Append($"{attribute.ProjectSettingsPath}/");
                        }

                        pathBuilder.Append(attribute.NameOverride ?? CoimbraEditorGUIUtility.ToDisplayName(type.Name));

                        if (attribute.IsEditorOnly)
                        {
                            dictionary.Add(type, new ScriptableSettingsProvider($"{pathBuilder}", type, $"{attribute.EditorFileDirectory}/{(attribute.EditorFileNameOverride ?? $"{type.Name}.asset")}", attribute.Keywords));
                        }
                        else
                        {
                            dictionary.Add(type, new ScriptableSettingsProvider($"{pathBuilder}", type, attribute.Keywords));
                        }

                        pathBuilder.Clear();
                    }

                    return dictionary.Values.ToArray();
                }
            }
        }
    }
}
