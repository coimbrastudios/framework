using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Drawer for <see cref="IntRange"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(IntRange))]
    [CustomPropertyDrawer(typeof(IntRangeAttribute))]
    public sealed class IntRangeDrawer : PropertyDrawer
    {
        /// <summary>
        /// Draws a <see cref="IntRange"/>. Optionally also makes it a delayed field.
        /// </summary>
        public static void DrawGUI(Rect position, SerializedProperty parentProperty, SerializedProperty minProperty, SerializedProperty maxProperty, GUIContent label, bool delayed)
        {
            using EditorGUI.PropertyScope propertyScope = new(position, label, parentProperty);
            position.height = EditorGUIUtility.singleLineHeight;

            Rect labelPosition = position;
            labelPosition.width = EditorGUIUtility.labelWidth;
            EditorGUI.LabelField(labelPosition, propertyScope.content);

            using (new EditorGUI.IndentLevelScope(-EditorGUI.indentLevel))
            {
                position.x += labelPosition.width;
                position.width -= labelPosition.width;
                DrawGUI(position, minProperty, maxProperty, delayed);
            }
        }

        /// <inheritdoc cref="DrawGUI(UnityEngine.Rect,UnityEditor.SerializedProperty,UnityEditor.SerializedProperty,UnityEditor.SerializedProperty,UnityEngine.GUIContent,bool)"/>
        public static void DrawGUI(Rect position, SerializedProperty minProperty, SerializedProperty maxProperty, bool delayed)
        {
            position.height = EditorGUIUtility.singleLineHeight;

            const float spacing = 2;
            const float labelWidth = 28;
            float totalWidth = position.width;
            float fieldWith = totalWidth / 2 - spacing / 2 - labelWidth;

            using (EditorGUI.PropertyScope propertyScope = new(position, new GUIContent(nameof(IntRange.Min)), minProperty))
            {
                position.width = labelWidth;
                EditorGUI.LabelField(position, propertyScope.content);

                position.x += position.width;
                position.width = fieldWith;

                using (EditorGUI.ChangeCheckScope changeCheckScope = new())
                {
                    int value = delayed
                                    ? EditorGUI.DelayedIntField(position, minProperty.intValue)
                                    : EditorGUI.IntField(position, minProperty.intValue);

                    if (changeCheckScope.changed)
                    {
                        minProperty.intValue = value;
                        maxProperty.intValue = Mathf.Max(value, maxProperty.intValue);
                    }
                }
            }

            using (EditorGUI.PropertyScope propertyScope = new(position, new GUIContent(nameof(IntRange.Max)), maxProperty))
            {
                position.x += position.width + spacing;
                position.width = labelWidth;
                EditorGUI.LabelField(position, propertyScope.content);

                position.x += position.width;
                position.width = fieldWith;

                using (EditorGUI.ChangeCheckScope changeCheckScope = new())
                {
                    int value = delayed
                                    ? EditorGUI.DelayedIntField(position, maxProperty.intValue)
                                    : EditorGUI.IntField(position, maxProperty.intValue);

                    if (changeCheckScope.changed)
                    {
                        maxProperty.intValue = Mathf.Max(value, minProperty.intValue);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty minProperty = property.FindPropertyRelative("_min") ?? property.FindPropertyRelative("x");

            if (minProperty == null || minProperty.propertyType != SerializedPropertyType.Integer)
            {
                EditorGUI.LabelField(position, label.text, $"{nameof(IntRangeAttribute)} requires a int property named 'x' or '_min'.");

                return;
            }

            SerializedProperty maxProperty = property.FindPropertyRelative("_max") ?? property.FindPropertyRelative("y");

            if (maxProperty == null || maxProperty.propertyType != SerializedPropertyType.Integer)
            {
                EditorGUI.LabelField(position, label.text, $"{nameof(IntRangeAttribute)} requires a int property named 'y' or '_max'.");

                return;
            }

            DrawGUI(position, property, minProperty, maxProperty, label, (attribute as IntRangeAttribute)?.Delayed ?? false);
        }
    }
}
