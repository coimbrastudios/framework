using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Drawer for <see cref="IntRange"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(IntRange))]
    [CustomPropertyDrawer(typeof(IntRangeAttribute))]
    public sealed class IntRangeDrawer : ValidateDrawer
    {
        /// <summary>
        /// Draws a <see cref="IntRange"/>. Optionally also makes it a delayed field.
        /// </summary>
        public static void DrawGUI(Rect position, SerializedProperty parentProperty, SerializedProperty minProperty, SerializedProperty maxProperty, GUIContent label, bool delayed)
        {
            using EditorGUI.PropertyScope propertyScope = new(position, label, parentProperty);
            position.height = EditorGUIUtility.singleLineHeight;
            position = EditorGUI.PrefixLabel(position, propertyScope.content);

            using (new ResetIndentLevelScope())
            {
                DrawGUI(position, minProperty, maxProperty, delayed);
            }
        }

        /// <inheritdoc cref="DrawGUI(UnityEngine.Rect,UnityEditor.SerializedProperty,UnityEditor.SerializedProperty,UnityEditor.SerializedProperty,UnityEngine.GUIContent,bool)"/>
        public static void DrawGUI(Rect position, SerializedProperty minProperty, SerializedProperty maxProperty, bool delayed)
        {
            const float labelWidth = 28;
            position.height = EditorGUIUtility.singleLineHeight;
            position.width *= 0.5f;
            position.width -= EditorGUIUtility.standardVerticalSpacing;

            using (new LabelWidthScope(labelWidth, LabelWidthScope.MagnitudeMode.Absolute))
            using (GUIContentPool.Pop(out GUIContent label))
            {
                label.text = nameof(IntRange.Min);

                using EditorGUI.PropertyScope propertyScope = new(position, label, minProperty);
                using EditorGUI.ChangeCheckScope changeCheckScope = new();

                int value = delayed
                                ? EditorGUI.DelayedIntField(position, propertyScope.content, minProperty.intValue)
                                : EditorGUI.IntField(position, propertyScope.content, minProperty.intValue);

                if (changeCheckScope.changed)
                {
                    minProperty.intValue = value;
                    maxProperty.intValue = Mathf.Max(value, maxProperty.intValue);
                }
            }

            position.x += position.width + EditorGUIUtility.standardVerticalSpacing;

            using (new LabelWidthScope(labelWidth, LabelWidthScope.MagnitudeMode.Absolute))
            using (GUIContentPool.Pop(out GUIContent label))
            {
                label.text = nameof(IntRange.Max);

                using EditorGUI.PropertyScope propertyScope = new(position, label, maxProperty);
                using EditorGUI.ChangeCheckScope changeCheckScope = new();

                int value = delayed
                                ? EditorGUI.DelayedIntField(position, propertyScope.content, maxProperty.intValue)
                                : EditorGUI.IntField(position, propertyScope.content, maxProperty.intValue);

                if (changeCheckScope.changed)
                {
                    maxProperty.intValue = Mathf.Max(value, minProperty.intValue);
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

            DrawGUI(position, property, minProperty, maxProperty, label, isDelayed);
        }
    }
}
