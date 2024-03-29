﻿#nullable enable

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

            internal bool IsPredefinedType;

            internal string? AssetPath;

            internal string? WindowPath;

            internal UnityEditor.Editor Editor;
        }

        private const string WindowsTitle = "Scriptable Settings";

        private static readonly string[] FilterOptions;

        private readonly Dictionary<Type, EditorState> _editorStates = new();

        [SerializeField]
        private int _filter;

        [SerializeField]
        private string _searchContext = string.Empty;

        [SerializeField]
        private Vector2 _scrollPosition;

        static ScriptableSettingsWindow()
        {
            FilterOptions = Enum.GetNames(typeof(ScriptableSettingsType));

            for (int i = 0; i < FilterOptions.Length; i++)
            {
                FilterOptions[i] = EngineUtility.GetDisplayName(FilterOptions[i]);
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
            using (new LabelWidthScope(EditorGUIUtility.currentViewWidth * ScriptableSettingsEditor.WindowLabelWidthPercent, LabelWidthScope.MagnitudeMode.Absolute))
            {
                using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
                {
                    using (new LabelWidthScope(35, LabelWidthScope.MagnitudeMode.Absolute))
                    {
                        _filter = EditorGUILayout.MaskField("Filter", _filter, FilterOptions, EditorStyles.toolbarPopup, GUILayout.Width(200));
                    }

                    _searchContext = EditorGUILayout.TextField(GUIContent.none, _searchContext, EditorStyles.toolbarSearchField);
                }

                using EditorGUILayout.ScrollViewScope scrollView = new(_scrollPosition);
                _scrollPosition = scrollView.scrollPosition;

                foreach (KeyValuePair<Type, ScriptableSettings?> pair in ScriptableSettings.Instances)
                {
                    if (_filter != 0 && (_filter & 1 << (int)ScriptableSettings.GetTypeData(pair.Key)) == 0)
                    {
                        continue;
                    }

                    if (!TryGetEditorState(pair.Key, pair.Value, out EditorState editorState) || pair.Value == null)
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
                    pair.Value.SaveAsset();
                }
            }
        }

        private void DrawEditor(Type type, ScriptableSettings value, ref EditorState editorState)
        {
            if (SplitSearchAt("t:", out string tokenValue, out string search) && !value.GetType().FullName.Contains(tokenValue, StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            if (editorState.Editor is ScriptableSettingsEditor scriptableSettingsEditor && !scriptableSettingsEditor.HasSearchInterest(search))
            {
                return;
            }

            using (new ScriptableSettingsSearchScope(search))
            using (new EditorGUI.DisabledScope(editorState is { IsPredefinedType: true, WindowPath: null }))
            {
                editorState.IsOpen = EditorGUILayout.InspectorTitlebar(editorState.IsOpen, editorState.Editor);

                if (!editorState.IsOpen)
                {
                    return;
                }

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
                            EditorGUI.SelectableLabel(rect, ApplicationUtility.GetPrefsKey(type));

                            break;
                        }

                        case ScriptableSettingsType.EditorProjectSettings:
                        case ScriptableSettingsType.ProjectUserPreferences:
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

        private bool SplitSearchAt(string tokenKey, out string tokenValue, out string search)
        {
            search = _searchContext.Trim();

            int tokenIndex = search.IndexOf(tokenKey, StringComparison.InvariantCultureIgnoreCase);

            if (tokenIndex < 0 || search.Length == tokenKey.Length)
            {
                tokenValue = string.Empty;
            }
            else if (tokenIndex == 0)
            {
                string[] split = search[tokenKey.Length..].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                search = split.Length > 1 ? split[1] : string.Empty;
                tokenValue = split[0];
            }
            else
            {
                string[] split = search.Split(tokenKey, StringSplitOptions.RemoveEmptyEntries);
                search = split[0];
                tokenValue = split.Length > 1 ? split[1].Split(' ', StringSplitOptions.RemoveEmptyEntries)[0] : string.Empty;
            }

            return !string.IsNullOrWhiteSpace(tokenValue);
        }

        private bool TryGetEditorState(Type type, ScriptableSettings? value, out EditorState editorState)
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

                    editorState.Editor = UnityEditor.Editor.CreateEditor(value);
                    editorState.IsPredefinedType = ScriptableSettings.GetTypeData(type, out editorState.WindowPath, out editorState.AssetPath, out _) != ScriptableSettingsType.Custom;
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
                editorState.IsPredefinedType = ScriptableSettings.GetTypeData(type, out editorState.WindowPath, out editorState.AssetPath, out _) != ScriptableSettingsType.Custom;
                _editorStates.Add(type, editorState);
            }

            return value != null;
        }
    }
}
