using UnityEditor;
using UnityEngine;

namespace CoimbraInternal.Editor
{
    internal static class UnityEditorInternals
    {
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
    }
}
