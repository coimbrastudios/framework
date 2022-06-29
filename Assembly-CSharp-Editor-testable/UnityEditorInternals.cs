using System;
using UnityEditor;
using UnityEngine;

namespace CoimbraInternal.Editor
{
    internal static class UnityEditorInternals
    {
        internal static event Action<bool> OnEditorApplicationFocusChanged
        {
            add => EditorApplication.focusChanged += value;
            remove => EditorApplication.focusChanged -= value;
        }

        internal static void ClearLogEntries()
        {
            LogEntries.Clear();
        }

        internal static void DrawSortingLayerField(Rect position, GUIContent label, SerializedProperty property, GUIStyle fieldStyle = null, GUIStyle labelStyle = null)
        {
            fieldStyle ??= EditorStyles.popup;
            labelStyle ??= EditorStyles.label;
            EditorGUI.SortingLayerField(position, label, property, fieldStyle, labelStyle);
        }

        internal static float GetIndent()
        {
            return EditorGUI.indent;
        }

        internal static float GetLineHeight()
        {
            return EditorGUI.lineHeight;
        }

        internal static Type GetTargetType(this CustomPropertyDrawer drawer)
        {
            return drawer.m_Type;
        }

        internal static bool GetUseForChildren(this CustomPropertyDrawer drawer)
        {
            return drawer.m_UseForChildren;
        }
    }
}
