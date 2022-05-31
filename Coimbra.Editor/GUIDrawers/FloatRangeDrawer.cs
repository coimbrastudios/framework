using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Drawer for <see cref="FloatRange"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(FloatRange))]
    [CustomPropertyDrawer(typeof(FloatRangeAttribute))]
    public sealed class FloatRangeDrawer : ValidateDrawer
    {
        /// <summary>
        /// Draws a <see cref="FloatRange"/>. Optionally also makes it a delayed field.
        /// </summary>
        public static void DrawGUI(Rect position, SerializedProperty parentProperty, SerializedProperty minProperty, SerializedProperty maxProperty, GUIContent label, bool delayed)
        {
            using EditorGUI.PropertyScope propertyScope = new EditorGUI.PropertyScope(position, label, parentProperty);
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

            using (GUIContentPool.Pop(out GUIContent label))
            {
                label.text = nameof(FloatRange.Min);

                using EditorGUI.PropertyScope propertyScope = new EditorGUI.PropertyScope(position, label, minProperty);

                {
                    position.width = labelWidth;
                    EditorGUI.LabelField(position, propertyScope.content);

                    position.x += position.width;
                    position.width = fieldWith;

                    using EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope();

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

            using (GUIContentPool.Pop(out GUIContent label))
            {
                label.text = nameof(FloatRange.Max);

                using EditorGUI.PropertyScope propertyScope = new EditorGUI.PropertyScope(position, label, maxProperty);

                {
                    position.x += position.width + spacing;
                    position.width = labelWidth;
                    EditorGUI.LabelField(position, propertyScope.content);

                    position.x += position.width;
                    position.width = fieldWith;

                    using EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope();

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

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        /// <inheritdoc/>
        protected override void DrawGUI(Rect position, SerializedProperty property, GUIContent label, PropertyPathInfo context, Object[] targets, bool isDelayed)
        {
            SerializedProperty minProperty = property.FindPropertyRelative("_min") ?? property.FindPropertyRelative("x");

            if (minProperty == null || minProperty.propertyType != SerializedPropertyType.Float)
            {
                EditorGUI.LabelField(position, label.text, $"{nameof(FloatRangeAttribute)} requires a float property named 'x' or '_min'.");

                return;
            }

            SerializedProperty maxProperty = property.FindPropertyRelative("_max") ?? property.FindPropertyRelative("y");

            if (maxProperty == null || maxProperty.propertyType != SerializedPropertyType.Float)
            {
                EditorGUI.LabelField(position, label.text, $"{nameof(FloatRangeAttribute)} requires a float property named 'y' or '_max'.");

                return;
            }

            DrawGUI(position, property, minProperty, maxProperty, label, isDelayed);
        }
    }
}
