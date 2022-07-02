using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Window to view all loaded <see cref="ScriptableSettings"/>.
    /// </summary>
    public sealed class ScriptableSettingsWindow : EditorWindow
    {
        private struct EditorState
        {
            internal bool IsOpen;

            internal SettingsScope? SettingsScope;

            internal string AssetPath;

            internal string WindowPath;

            internal UnityEditor.Editor Editor;
        }

        private const string WindowsTitle = "Scriptable Settings";

        private static readonly string[] FilterOptions;

        private readonly Dictionary<Type, EditorState> _editorStates = new();

        [SerializeField]
        private int _filter;

        [SerializeField]
        private Vector2 _scrollPosition;

        static ScriptableSettingsWindow()
        {
            FilterOptions = Enum.GetNames(typeof(ScriptableSettingsType));

            for (int i = 0; i < FilterOptions.Length; i++)
            {
                FilterOptions[i] = CoimbraGUIUtility.GetDisplayName(FilterOptions[i]);
            }
        }

        /// <summary>
        /// Opens the <see cref="ScriptableSettingsWindow"/>.
        /// </summary>
        [MenuItem(CoimbraUtility.WindowMenuPath + WindowsTitle)]
        public static void Open()
        {
            GetWindow<ScriptableSettingsWindow>(WindowsTitle);
        }

        private void OnDisable()
        {
            foreach (EditorState editorState in _editorStates.Values)
            {
                if (editorState.Editor != null)
                {
                    DestroyImmediate(editorState.Editor);
                }
            }

            _editorStates.Clear();
        }

        private void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                _filter = EditorGUILayout.MaskField("Filter", _filter, FilterOptions, EditorStyles.toolbarPopup);
            }

            using EditorGUILayout.ScrollViewScope scrollView = new(_scrollPosition);
            _scrollPosition = scrollView.scrollPosition;

            foreach (KeyValuePair<Type, ScriptableSettings> pair in ScriptableSettings.Map)
            {
                if (_filter != 0 && (_filter & 1 << (int)ScriptableSettings.GetType(pair.Key)) == 0)
                {
                    continue;
                }

                if (!TryGetEditorState(pair.Key, pair.Value, out EditorState editorState))
                {
                    continue;
                }

                using EditorGUI.ChangeCheckScope changeCheckScope = new();

                DrawEditor(pair.Key, pair.Value, ref editorState);

                if (!changeCheckScope.changed)
                {
                    continue;
                }

                _editorStates[pair.Key] = editorState;
                pair.Value.Save();
            }
        }

        private void DrawEditor(Type type, ScriptableSettings value, ref EditorState editorState)
        {
            using (new EditorGUI.DisabledScope(editorState.SettingsScope != null && editorState.WindowPath == null))
            {
                editorState.IsOpen = EditorGUILayout.InspectorTitlebar(editorState.IsOpen, editorState.Editor);

                if (editorState.IsOpen)
                {
                    using (GUIContentPool.Pop(out GUIContent label))
                    {
                        using (new EditorGUI.DisabledScope(true))
                        {
                            EditorGUILayout.EnumPopup("Type", value.Type);
                        }

                        switch (value.Type)
                        {
                            case ScriptableSettingsType.Custom:
                            case ScriptableSettingsType.RuntimeProjectSettings:
                            {
                                using (new EditorGUI.DisabledScope(true))
                                {
                                    EditorGUILayout.ObjectField("Asset", value, type, false);
                                }

                                break;
                            }

                            case ScriptableSettingsType.EditorUserPreferences:
                            {
                                label.text = "Asset Key";

                                Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
                                rect = EditorGUI.PrefixLabel(rect, label);
                                EditorGUI.SelectableLabel(rect, ScriptableSettingsUtility.GetPrefsKey(type));

                                break;
                            }

                            default:
                            {
                                label.text = "Asset Path";

                                Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
                                rect = EditorGUI.PrefixLabel(rect, label);
                                EditorGUI.SelectableLabel(rect, editorState.AssetPath);

                                break;
                            }
                        }
                    }

                    editorState.Editor.OnInspectorGUI();
                }
            }
        }

        private bool TryGetEditorState(Type type, ScriptableSettings value, out EditorState editorState)
        {
            if (_editorStates.TryGetValue(type, out editorState))
            {
                if (editorState.Editor.target != value)
                {
                    DestroyImmediate(editorState.Editor);

                    if (value == null)
                    {
                        _editorStates.Remove(type);

                        return false;
                    }

                    ScriptableSettingsUtility.TryGetAttributeData(type, out editorState.SettingsScope, out editorState.WindowPath, out editorState.AssetPath, out _);

                    editorState.Editor = UnityEditor.Editor.CreateEditor(value);
                    _editorStates[type] = editorState;
                }
            }
            else
            {
                if (value == null)
                {
                    return false;
                }

                editorState.Editor = UnityEditor.Editor.CreateEditor(value);
                ScriptableSettingsUtility.TryGetAttributeData(type, out editorState.SettingsScope, out editorState.WindowPath, out editorState.AssetPath, out _);
                _editorStates.Add(type, editorState);
            }

            return true;
        }
    }
}
