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
        private readonly string? _editorFilePath;

        private readonly Type _type;

        public ScriptableSettingsProvider(string settingsWindowPath, Type type, string editorFilePath, IEnumerable<string>? keywords)
            : base(settingsWindowPath, () => UnityEditor.Editor.CreateEditor(CreateOrLoadScriptableSettings(type, editorFilePath)), keywords)
        {
            Debug.Assert(typeof(ScriptableSettings).IsAssignableFrom(type));
            _editorFilePath = editorFilePath;
            _type = type;
        }

        public ScriptableSettingsProvider(string settingsWindowPath, Type type, IEnumerable<string>? keywords)
            : base(settingsWindowPath, () => UnityEditor.Editor.CreateEditor(ScriptableSettings.GetOrFind(type)), keywords)
        {
            Debug.Assert(typeof(ScriptableSettings).IsAssignableFrom(type));
            _editorFilePath = null;
            _type = type;
        }

        public ScriptableSettingsProvider(string settingsWindowPath, Type type)
            : base(settingsWindowPath, () => ScriptableSettings.GetOrFind(type))
        {
            Debug.Assert(typeof(ScriptableSettings).IsAssignableFrom(type));
            _editorFilePath = null;
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

                if (!changeCheckScope.changed || _editorFilePath == null)
                {
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
            else
            {
                if (_editorFilePath == null && GUILayout.Button($"Create {_type.Name} asset", GUILayout.Height(30)))
                {
                    CreateScriptableSettings();
                }
            }
        }

        private static ScriptableSettings CreateOrLoadScriptableSettings(Type type, string filePath)
        {
            if (ScriptableSettings.TryGetOrFind(type, out ScriptableSettings value))
            {
                return value;
            }

            if (!string.IsNullOrEmpty(filePath))
            {
                Object[] objects = InternalEditorUtility.LoadSerializedFileAndForget(filePath);

                foreach (Object o in objects)
                {
                    if (!(o is ScriptableSettings match))
                    {
                        continue;
                    }

                    ScriptableSettings.Set(type, match);

                    return match;
                }
            }

            value = (ScriptableSettings)ScriptableObject.CreateInstance(type);
            ScriptableSettings.Set(type, value);

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
                UnityEditor.Editor.CreateEditor(settings)
            });
        }
    }
}
