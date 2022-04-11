using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Specialization of <see cref="AssetSettingsProvider"/> to use with <see cref="ScriptableSettings"/>.
    /// </summary>
    public class ScriptableSettingsProvider : AssetSettingsProvider
    {
        private readonly Type _type;

        public ScriptableSettingsProvider(string settingsWindowPath, Type type, IEnumerable<string> keywords = null)
            : base(settingsWindowPath, () => UnityEditor.Editor.CreateEditor(ScriptableSettings.GetOrFind(type, ScriptableSettings.FindFirst)), keywords)
        {
            Debug.Assert(typeof(ScriptableSettings).IsAssignableFrom(type));
            _type = type;
        }

        public ScriptableSettingsProvider(string settingsWindowPath, Type type)
            : base(settingsWindowPath, () => ScriptableSettings.GetOrFind(type, ScriptableSettings.FindFirst))
        {
            Debug.Assert(typeof(ScriptableSettings).IsAssignableFrom(type));
            _type = type;
        }

        /// <inheritdoc/>
        public override void OnGUI(string searchContext)
        {
            if (settingsEditor == null)
            {
                if (GUILayout.Button($"Create {_type.Name} asset", GUILayout.Height(30)))
                {
                    CreateScriptableSettings();
                }

                EditorGUILayout.Space();
            }

            base.OnGUI(searchContext);
        }

        private void CreateScriptableSettings()
        {
            string path = EditorUtility.SaveFilePanel($"Create {_type.Name} File", "Assets", $"{_type.Name}.asset", "asset");

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

            string extension = Path.GetExtension(path);

            if (string.Compare(extension, ".asset", StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                path += ".asset";
            }

            string relativePath = "Assets/" + path.Substring(dataPath.Length);
            ScriptableSettings settings = (ScriptableSettings)ScriptableObject.CreateInstance(_type);
            AssetDatabase.CreateAsset(settings, relativePath);
            EditorGUIUtility.PingObject(settings);
            ScriptableSettings.Set(_type, settings);
        }
    }
}
