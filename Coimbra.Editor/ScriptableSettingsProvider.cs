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
            : base(settingsWindowPath, () => UnityEditor.Editor.CreateEditor(ScriptableSettings.GetOrFind(type)), keywords)
        {
            ScriptableSettingsUtility.LoadOrCreate(type);

            FieldInfo? field = typeof(SettingsProvider).GetField($"<{nameof(scope)}>k__BackingField", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Debug.Assert(field != null);
            field!.SetValue(this, scope);

            Debug.Assert(typeof(ScriptableSettings).IsAssignableFrom(type));
            _editorFilePath = editorFilePath;
            _type = type;
        }

        /// <summary>
        /// Gets the current set search context.
        /// </summary>
        public static string? CurrentSearchContext { get; private set; }

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
                using EditorGUI.ChangeCheckScope changeCheckScope = new();
                CurrentSearchContext = searchContext;

                if (!keywords.Any() || TryMatchKeywords(searchContext))
                {
                    base.OnGUI(searchContext);
                }

                CurrentSearchContext = null;

                if (changeCheckScope.changed)
                {
                    settings.Save();
                }
            }
            else if (ScriptableSettings.TryGetOrFind(_type, out settings))
            {
                SetSettingsEditor(UnityEditor.Editor.CreateEditor(settings));
            }
            else if (scope == SettingsScope.Project && _editorFilePath == null && GUILayout.Button($"Create {_type.Name} asset", GUILayout.Height(30)))
            {
                CreateScriptableSettings();
            }
        }

        /// <inheritdoc/>
        public override void OnTitleBarGUI()
        {
            if (scope != SettingsScope.Project || _editorFilePath != null)
            {
                if (settingsEditor == null)
                {
                    ScriptableSettings target = ScriptableSettingsUtility.LoadOrCreate(_type)!;
                    SetSettingsEditor(UnityEditor.Editor.CreateEditor(target));
                }
                else if (settingsEditor.target == null)
                {
                    Object.DestroyImmediate(settingsEditor);

                    ScriptableSettings target = ScriptableSettingsUtility.LoadOrCreate(_type)!;
                    SetSettingsEditor(UnityEditor.Editor.CreateEditor(target));
                }
            }

            if (settingsEditor != null && settingsEditor.target != null)
            {
                base.OnTitleBarGUI();
            }
        }

        internal static bool TryCreate(Type type, [NotNullWhen(true)] out ScriptableSettingsProvider? provider)
        {
            ScriptableSettingsUtility.TryGetAttributeData(type, out SettingsScope? settingsScope, out string? windowPath, out string? filePath, out string[]? keywords);
            Debug.Assert(settingsScope.HasValue);

            provider = windowPath != null ? new ScriptableSettingsProvider(windowPath, type, settingsScope!.Value, filePath, keywords) : null;

            return provider != null;
        }

        private void CreateScriptableSettings()
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
            ScriptableSettings settings = (ScriptableSettings)ScriptableObject.CreateInstance(_type);
            AssetDatabase.CreateAsset(settings, relativePath);
            EditorGUIUtility.PingObject(settings);
            ScriptableSettings.Set(_type, settings);
            SetSettingsEditor(UnityEditor.Editor.CreateEditor(settings));
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
                if (CoimbraGUIUtility.TryMatchSearch(searchContext, keyword))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
