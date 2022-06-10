#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
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

        /// <inheritdoc/>
        public override void OnFooterBarGUI()
        {
            if (settingsEditor != null && settingsEditor.target != null)
            {
                base.OnFooterBarGUI();
            }
        }

        /// <inheritdoc/>
        public override void OnTitleBarGUI()
        {
            if (settingsEditor != null && settingsEditor.target != null)
            {
                base.OnTitleBarGUI();
            }
        }

        /// <inheritdoc/>
        public override void OnGUI(string searchContext)
        {
            if (settingsEditor != null && settingsEditor.target != null && settingsEditor.target is ScriptableSettings settings)
            {
                using EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope();

                base.OnGUI(searchContext);

                if (changeCheckScope.changed)
                {
                    settings.Save();
                }
            }
            else
            {
                if (scope == SettingsScope.Project && _editorFilePath == null && GUILayout.Button($"Create {_type.Name} asset", GUILayout.Height(30)))
                {
                    CreateScriptableSettings();
                }
            }
        }

        internal static ScriptableSettingsProvider Create(Type type)
        {
            ScriptableSettingsUtility.TryGetAttributeData(type, out SettingsScope? settingsScope, out string? windowPath, out string? filePath, out string[]? keywords);
            Debug.Assert(settingsScope.HasValue);

            return new ScriptableSettingsProvider(windowPath!, type, settingsScope!.Value, filePath, keywords);
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

            if (string.Compare(extension, ".asset", StringComparison.InvariantCultureIgnoreCase) != 0)
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
                value
            });
        }
    }
}
