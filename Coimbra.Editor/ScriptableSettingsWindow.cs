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

        private readonly Dictionary<Type, EditorState> _editorStates = new Dictionary<Type, EditorState>();

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

            using EditorGUILayout.ScrollViewScope scrollView = new EditorGUILayout.ScrollViewScope(_scrollPosition);
            _scrollPosition = scrollView.scrollPosition;

            foreach (KeyValuePair<Type, ScriptableSettings> pair in ScriptableSettings.Map)
            {
                if (_editorStates.TryGetValue(pair.Key, out EditorState editorState))
                {
                    if (editorState.Editor.target != pair.Value)
                    {
                        DestroyImmediate(editorState.Editor);

                        if (pair.Value == null)
                        {
                            _editorStates.Remove(pair.Key);

                            continue;
                        }

                        ScriptableSettingsUtility.TryGetAttributeData(pair.Key, out editorState.SettingsScope, out editorState.WindowPath, out editorState.AssetPath, out _);

                        editorState.Editor = UnityEditor.Editor.CreateEditor(pair.Value);
                        _editorStates[pair.Key] = editorState;
                    }
                }
                else
                {
                    if (pair.Value == null)
                    {
                        continue;
                    }

                    editorState.Editor = UnityEditor.Editor.CreateEditor(pair.Value);
                    ScriptableSettingsUtility.TryGetAttributeData(pair.Key, out editorState.SettingsScope, out editorState.WindowPath, out editorState.AssetPath, out _);
                    _editorStates.Add(pair.Key, editorState);
                }

                if (_filter != 0 && (_filter & 1 << (int)ScriptableSettings.GetType(pair.Key)) == 0)
                {
                    continue;
                }

                using EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope();

                using (new EditorGUI.DisabledScope(editorState.SettingsScope != null && editorState.WindowPath == null))
                {
                    editorState.IsOpen = EditorGUILayout.InspectorTitlebar(editorState.IsOpen, editorState.Editor);

                    if (editorState.IsOpen)
                    {
                        using (GUIContentPool.Pop(out GUIContent label))
                        {
                            using (new EditorGUI.DisabledScope(true))
                            {
                                EditorGUILayout.EnumPopup("Type", pair.Value.Type);
                            }

                            switch (pair.Value.Type)
                            {
                                case ScriptableSettingsType.Custom:
                                case ScriptableSettingsType.RuntimeProjectSettings:
                                {
                                    using (new EditorGUI.DisabledScope(true))
                                    {
                                        EditorGUILayout.ObjectField("Asset", pair.Value, pair.Key, false);
                                    }

                                    break;
                                }

                                case ScriptableSettingsType.EditorUserPreferences:
                                {
                                    label.text = "Asset Key";

                                    Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
                                    rect = EditorGUI.PrefixLabel(rect, label);
                                    EditorGUI.SelectableLabel(rect, ScriptableSettingsUtility.GetPrefsKey(pair.Key));

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
