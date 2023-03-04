#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Coimbra.Editor
{
    /// <summary>
    /// Specialization of <see cref="AssetSettingsProvider"/> to use with <see cref="ScriptableSettings"/>.
    /// </summary>
    public sealed class ScriptableSettingsProvider : AssetSettingsProvider
    {
        private readonly string? _editorFilePath;

        private readonly Type _type;

        private ScriptableSettingsProvider(string settingsWindowPath, Type type, SettingsScope scope, string? editorFilePath, IEnumerable<string>? keywords)
            : base(settingsWindowPath, () => UnityEditor.Editor.CreateEditor(ScriptableSettings.Get(type)), keywords)
        {
            ScriptableSettings.Get(type);

            FieldInfo? field = typeof(SettingsProvider).GetField($"<{nameof(scope)}>k__BackingField", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Debug.Assert(field != null);
            field!.SetValue(this, scope);

            Debug.Assert(typeof(ScriptableSettings).IsAssignableFrom(type));
            _editorFilePath = editorFilePath;
            _type = type;
        }

        /// <inheritdoc/>
        public override void OnFooterBarGUI()
        {
            if (settingsEditor != null && settingsEditor.target != null)
            {
                base.OnFooterBarGUI();
            }
        }

        /// <inheritdoc/>
        public override bool HasSearchInterest(string searchContext)
        {
            if (base.HasSearchInterest(searchContext))
            {
                return true;
            }

            return settingsEditor != null && settingsEditor is ScriptableSettingsEditor editor && editor.HasSearchInterest(searchContext);
        }

        /// <inheritdoc/>
        public override void OnGUI(string searchContext)
        {
            if (settingsEditor != null && settingsEditor.target != null && settingsEditor.target is ScriptableSettings settings)
            {
                bool requiresCreation = RequiresCreation(settings);

                if (requiresCreation)
                {
                    if (GUILayout.Button($"Create {_type.Name} asset", GUILayout.Height(30)))
                    {
                        CreateAsset(settings);
                    }
                }

                using EditorGUI.ChangeCheckScope changeCheckScope = new();

                using (new ScriptableSettingsSearchScope(searchContext))
                using (new EditorGUI.DisabledScope(requiresCreation))
                {
                    if (!keywords.Any() || TryMatchKeywords(searchContext))
                    {
                        base.OnGUI(searchContext);
                    }
                }

                if (changeCheckScope.changed)
                {
                    settings.SaveAsset();
                }
            }
            else
            {
                SetSettingsEditor(UnityEditor.Editor.CreateEditor(ScriptableSettings.Get(_type)));
            }
        }

        /// <inheritdoc/>
        public override void OnTitleBarGUI()
        {
            if (scope != SettingsScope.Project || _editorFilePath != null)
            {
                if (settingsEditor == null)
                {
                    SetSettingsEditor(UnityEditor.Editor.CreateEditor(ScriptableSettings.Get(_type)));
                }
                else if (settingsEditor.target == null)
                {
                    Object.DestroyImmediate(settingsEditor);
                    SetSettingsEditor(UnityEditor.Editor.CreateEditor(ScriptableSettings.Get(_type)));
                }
            }

            if (settingsEditor != null && settingsEditor.target != null)
            {
                base.OnTitleBarGUI();
            }
        }

        internal static ScriptableSettingsProvider CreateProjectSettingsProvider(Type type)
        {
            ScriptableSettings.GetTypeData(type, out string? windowPath, out string? filePath, out string[]? keywords);
            Debug.Assert(windowPath != null, $"{type} should not have null as the {nameof(ProjectSettingsAttribute.WindowPath)}.");

            return new ScriptableSettingsProvider(windowPath!, type, SettingsScope.Project, filePath, keywords);
        }

        internal static bool TryCreatePreferencesProvider(Type type, [NotNullWhen(true)] out ScriptableSettingsProvider? provider)
        {
            ScriptableSettings.GetTypeData(type, out string? windowPath, out string? filePath, out string[]? keywords);
            provider = windowPath != null ? new ScriptableSettingsProvider(windowPath, type, SettingsScope.User, filePath, keywords) : null;

            return provider != null;
        }

        private void CreateAsset(ScriptableSettings settings)
        {
            string? path = EditorUtility.SaveFilePanel($"Create {_type.Name} File", "Assets", $"{_type.Name}.asset", "asset");

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            path = path.Replace("\\", "/");

            string dataPath = Application.dataPath + "/";

            if (!path.StartsWith(dataPath, StringComparison.CurrentCultureIgnoreCase))
            {
                Debug.LogError($"{_type.Name} must be stored in Assets folder of the project (got: '{path}')");

                return;
            }

            string? extension = Path.GetExtension(path);

            if (StringComparer.InvariantCultureIgnoreCase.Compare(extension, ".asset") != 0)
            {
                path += ".asset";
            }

            string relativePath = "Assets/" + path.Substring(dataPath.Length);

            AssetDatabase.CreateAsset(settings, relativePath);
            EditorGUIUtility.PingObject(settings);
        }

        private bool RequiresCreation(Object settings)
        {
            return scope == SettingsScope.Project && string.IsNullOrWhiteSpace(_editorFilePath) && string.IsNullOrWhiteSpace(AssetDatabase.GetAssetPath(settings));
        }

        private void SetSettingsEditor(UnityEditor.Editor value)
        {
            if (settingsEditor != null)
            {
                Object.DestroyImmediate(settingsEditor);
            }

            PropertyInfo? property = typeof(AssetSettingsProvider).GetProperty(nameof(settingsEditor), BindingFlags.Instance | BindingFlags.Public);
            Debug.Assert(property != null);

            MethodInfo? setter = property!.GetSetMethod(true);
            Debug.Assert(setter != null);

            setter!.Invoke(this, new object[]
            {
                value,
            });
        }

        private bool TryMatchKeywords(string searchContext)
        {
            foreach (string keyword in keywords)
            {
                if (EngineUtility.TryMatchSearch(searchContext, keyword))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
