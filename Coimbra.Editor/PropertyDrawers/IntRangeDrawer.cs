using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Drawer for <see cref="IntRange"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(IntRange))]
    public sealed class IntRangeDrawer : PropertyDrawer
    {
        private const string MaxSerializedProperty = "_max";

        private const string MinSerializedProperty = "_min";

        /// <summary>
        /// Draws a <see cref="IntRange"/>. Optionally also makes it a delayed field.
        /// </summary>
        public static void DrawGUI(Rect position, SerializedProperty property, GUIContent label, bool delayed)
        {
            using EditorGUI.PropertyScope propertyScope = new EditorGUI.PropertyScope(position, label, property);

            position.height = EditorGUIUtility.singleLineHeight;

            Rect labelPosition = position;
            labelPosition.width = EditorGUIUtility.labelWidth;
            EditorGUI.LabelField(labelPosition, propertyScope.content);

            using (new EditorGUI.IndentLevelScope(-EditorGUI.indentLevel))
            {
                position.x += labelPosition.width;
                position.width -= labelPosition.width;

                SerializedProperty minProperty = property.FindPropertyRelative(MinSerializedProperty);
                SerializedProperty maxProperty = property.FindPropertyRelative(MaxSerializedProperty);
                DrawGUI(position, minProperty, maxProperty, delayed);
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
            DrawGUI(position, property, label, false);
        }

        private static void DrawGUI(Rect position, SerializedProperty minProperty, SerializedProperty maxProperty, bool delayed)
        {
            position.height = EditorGUIUtility.singleLineHeight;

            const float spacing = 2;
            const float labelWidth = 28;
            float totalWidth = position.width;
            float fieldWith = totalWidth / 2 - spacing / 2 - labelWidth;

            using (EditorGUI.PropertyScope propertyScope = new EditorGUI.PropertyScope(position, new GUIContent(nameof(IntRange.Min)), minProperty))
            {
                position.width = labelWidth;
                EditorGUI.LabelField(position, propertyScope.content);

                position.x += position.width;
                position.width = fieldWith;

                using (EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope())
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

            using (EditorGUI.PropertyScope propertyScope = new EditorGUI.PropertyScope(position, new GUIContent(nameof(IntRange.Max)), maxProperty))
            {
                position.x += position.width + spacing;
                position.width = labelWidth;
                EditorGUI.LabelField(position, propertyScope.content);

                position.x += position.width;
                position.width = fieldWith;

                using (EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope())
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
    }
}
