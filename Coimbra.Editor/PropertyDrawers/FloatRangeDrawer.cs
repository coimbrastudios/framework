using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Drawer for <see cref="FloatRange"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(FloatRange))]
    public sealed class FloatRangeDrawer : PropertyDrawer
    {
        private const string MaxSerializedProperty = "_max";

        private const string MinSerializedProperty = "_min";

        /// <summary>
        /// Draws a <see cref="FloatRange"/>. Optionally also makes it a delayed field.
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

            using (EditorGUI.PropertyScope propertyScope = new EditorGUI.PropertyScope(position, new GUIContent(nameof(FloatRange.Min)), minProperty))
            {
                position.width = labelWidth;
                EditorGUI.LabelField(position, propertyScope.content);

                position.x += position.width;
                position.width = fieldWith;

                using (EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    float value = delayed
                                      ? EditorGUI.DelayedFloatField(position, minProperty.floatValue)
                                      : EditorGUI.FloatField(position, minProperty.floatValue);

                    if (changeCheckScope.changed)
                    {
                        minProperty.floatValue = value;
                        maxProperty.floatValue = Mathf.Max(value, maxProperty.floatValue);
                    }
                }
            }

            using (EditorGUI.PropertyScope propertyScope = new EditorGUI.PropertyScope(position, new GUIContent(nameof(FloatRange.Max)), maxProperty))
            {
                position.x += position.width + spacing;
                position.width = labelWidth;
                EditorGUI.LabelField(position, propertyScope.content);

                position.x += position.width;
                position.width = fieldWith;

                using (EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    float value = delayed
                                      ? EditorGUI.DelayedFloatField(position, maxProperty.floatValue)
                                      : EditorGUI.FloatField(position, maxProperty.floatValue);

                    if (changeCheckScope.changed)
                    {
                        maxProperty.floatValue = Mathf.Max(value, minProperty.floatValue);
                    }
                }
            }
        }
    }
}
