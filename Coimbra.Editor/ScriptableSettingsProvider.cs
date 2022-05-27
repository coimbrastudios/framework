#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Coimbra.Editor
{
    /// <summary>
    /// Specialization of <see cref="AssetSettingsProvider"/> to use with <see cref="ScriptableSettings"/>.
    /// </summary>
    public class ScriptableSettingsProvider : AssetSettingsProvider
    {
        private const string EditorPrefsFormat = "Coimbra.Editor.ScriptableSettingsProvider.{0}";

        private readonly string? _editorFilePath;

        private readonly Type _type;

        protected ScriptableSettingsProvider(string settingsWindowPath, Type type, SettingsScope scope, string? editorFilePath)
            : base(settingsWindowPath, () => ScriptableSettings.GetOrFind(type))
        {
            CreateOrLoadScriptableSettings(type, editorFilePath, scope);

            FieldInfo? field = typeof(SettingsProvider).GetField($"<{nameof(scope)}>k__BackingField", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Debug.Assert(field != null);
            field!.SetValue(this, scope);

            Debug.Assert(typeof(ScriptableSettings).IsAssignableFrom(type));
            _editorFilePath = editorFilePath;
            _type = type;
        }

        protected ScriptableSettingsProvider(string settingsWindowPath, Type type, SettingsScope scope, string? editorFilePath, IEnumerable<string>? keywords)
            : base(settingsWindowPath, () => UnityEditor.Editor.CreateEditor(CreateOrLoadScriptableSettings(type, editorFilePath, scope)), keywords)
        {
            CreateOrLoadScriptableSettings(type, editorFilePath, scope);

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
            if (settingsEditor != null && settingsEditor.target != null)
            {
                using EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope();

                base.OnGUI(searchContext);

                if (!changeCheckScope.changed)
                {
                    SaveScriptableObjectSettings();
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

        internal static ScriptableSettingsProvider CreatePreferencesProvider(string settingsWindowPath, Type type, string? editorFilePath, IEnumerable<string>? keywords)
        {
            return new ScriptableSettingsProvider(settingsWindowPath, type, SettingsScope.User, editorFilePath, keywords);
        }

        internal static ScriptableSettingsProvider CreateProjectSettingsProvider(string settingsWindowPath, Type type, string? editorFilePath, IEnumerable<string>? keywords)
        {
            return new ScriptableSettingsProvider(settingsWindowPath, type, SettingsScope.Project, editorFilePath, keywords);
        }

        private static ScriptableSettings? CreateOrLoadScriptableSettings(Type type, string? filePath, SettingsScope scope)
        {
            ScriptableSettingsType filter = ScriptableSettings.GetType(type);

            if (ScriptableSettings.TryGetOrFind(type, out ScriptableSettings value) && value.Type == filter)
            {
                return value;
            }

            if (!string.IsNullOrEmpty(filePath))
            {
                Object[] objects = InternalEditorUtility.LoadSerializedFileAndForget(filePath);

                foreach (Object o in objects)
                {
                    if (o is ScriptableSettings match && match.Type == filter)
                    {
                        ScriptableSettings.Set(type, match);

                        return match;
                    }
                }

                value = (ScriptableSettings)ScriptableObject.CreateInstance(type);
            }
            else if (scope == SettingsScope.User)
            {
                value = (ScriptableSettings)ScriptableObject.CreateInstance(type);

                string defaultValue = EditorJsonUtility.ToJson(value, false);
                string newValue = EditorPrefs.GetString(string.Format(EditorPrefsFormat, type.FullName), defaultValue);
                EditorJsonUtility.FromJsonOverwrite(newValue, value);

                if (value.Type != filter)
                {
                    value = (ScriptableSettings)ScriptableObject.CreateInstance(type);
                }
            }

            ScriptableSettings.SetOrOverwrite(type, value);

            return value;
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

        private void SaveScriptableObjectSettings()
        {
            if (_editorFilePath == null)
            {
                if (scope == SettingsScope.Project)
                {
                    return;
                }

                string value = EditorJsonUtility.ToJson(settingsEditor.target, true);
                EditorPrefs.SetString(string.Format(EditorPrefsFormat, _type.FullName), value);

                return;
            }

            string? directoryName = Path.GetDirectoryName(_editorFilePath);

            if (!string.IsNullOrWhiteSpace(directoryName) && !Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            InternalEditorUtility.SaveToSerializedFileAndForget(new Object[]
            {
                ScriptableSettings.GetOrFind(_type)
            }, _editorFilePath, true);
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
