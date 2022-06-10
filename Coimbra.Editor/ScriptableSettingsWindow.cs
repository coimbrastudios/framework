using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    public sealed class ScriptableSettingsWindow : EditorWindow
    {
        private const string WindowsTitle = "Scriptable Settings";

        private static readonly string[] FilterOptions;

        private readonly Dictionary<Type, (bool IsOpen, UnityEditor.Editor Value)> _editorStates = new Dictionary<Type, (bool, UnityEditor.Editor)>();

        [SerializeField]
        private int _filter;

        [SerializeField]
        private Vector2 _scrollPosition;

        static ScriptableSettingsWindow()
        {
            FilterOptions = Enum.GetNames(typeof(ScriptableSettingsType));

            for (int i = 0; i < FilterOptions.Length; i++)
            {
                FilterOptions[i] = CoimbraEditorGUIUtility.ToDisplayName(FilterOptions[i]);
            }
        }

        [MenuItem(CoimbraUtility.WindowsMenuPath + WindowsTitle)]
        private static void Open()
        {
            GetWindow<ScriptableSettingsWindow>(WindowsTitle);
        }

        private void OnDisable()
        {
            foreach ((bool IsOpen, UnityEditor.Editor Value) editorState in _editorStates.Values)
            {
                if (editorState.Value != null)
                {
                    DestroyImmediate(editorState.Value);
                }
            }

            _editorStates.Clear();
        }

        private void OnGUI()
        {
            using EditorGUILayout.ScrollViewScope scrollView = new EditorGUILayout.ScrollViewScope(_scrollPosition);
            _scrollPosition = scrollView.scrollPosition;
            _filter = EditorGUILayout.MaskField("Filter", _filter, FilterOptions);

            foreach (KeyValuePair<Type, ScriptableSettings> pair in ScriptableSettings.Values)
            {
                if (_editorStates.TryGetValue(pair.Key, out (bool IsOpen, UnityEditor.Editor Value) editorState))
                {
                    if (editorState.Value.target != pair.Value)
                    {
                        DestroyImmediate(editorState.Value);

                        if (pair.Value == null)
                        {
                            _editorStates.Remove(pair.Key);

                            continue;
                        }

                        editorState.Value = UnityEditor.Editor.CreateEditor(pair.Value);
                        _editorStates[pair.Key] = editorState;
                    }
                }
                else
                {
                    if (pair.Value == null)
                    {
                        continue;
                    }

                    editorState.Value = UnityEditor.Editor.CreateEditor(pair.Value);
                    _editorStates.Add(pair.Key, editorState);
                }

                if (_filter != 0 && (_filter & 1 << (int)ScriptableSettings.GetType(pair.Key)) == 0)
                {
                    continue;
                }

                using EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope();
                editorState.IsOpen = EditorGUILayout.InspectorTitlebar(editorState.IsOpen, editorState.Value);

                if (editorState.IsOpen)
                {
                    editorState.Value.OnInspectorGUI();
                }

                if (!changeCheckScope.changed)
                {
                    continue;
                }

                _editorStates[pair.Key] = editorState;
                pair.Value.Save();
            }
        }
    }
}
