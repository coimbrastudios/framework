using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Drawer for <see cref="RangeSliderAttribute"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(RangeSliderAttribute))]
    public sealed class RangeSliderDrawer : ValidateDrawer
    {
        /// <summary>
        /// Draws a MinMaxSlider. Optionally also makes it a delayed field.
        /// </summary>
        public static void DrawGUI(Rect position, SerializedProperty parentProperty, SerializedProperty minProperty, SerializedProperty maxProperty, GUIContent label, float minLimit, float maxLimit, bool roundToInt, bool delayed)
        {
            using EditorGUI.PropertyScope propertyScope = new(position, label, parentProperty);
            position.height = EditorGUIUtility.singleLineHeight;
            position = EditorGUI.PrefixLabel(position, propertyScope.content);

            using (new ResetIndentLevelScope())
            {
                if (roundToInt)
                {
                    DrawGUI(position, minProperty, maxProperty, Mathf.CeilToInt(minLimit), Mathf.FloorToInt(maxLimit), delayed);
                }
                else
                {
                    DrawGUI(position, minProperty, maxProperty, minLimit, maxLimit, delayed);
                }
            }
        }

        /// <inheritdoc cref="DrawGUI(UnityEngine.Rect,UnityEditor.SerializedProperty,UnityEditor.SerializedProperty,UnityEditor.SerializedProperty,UnityEngine.GUIContent,float,float,bool,bool)"/>
        public static void DrawGUI(Rect position, SerializedProperty minProperty, SerializedProperty maxProperty, int minLimit, int maxLimit, bool delayed)
        {
            position.height = EditorGUIUtility.singleLineHeight;

            const float fieldWidth = 50;
            float totalWidth = position.width;

            using (new EditorGUI.PropertyScope(position, GUIContent.none, minProperty))
            {
                position.width = fieldWidth;

                using (EditorGUI.ChangeCheckScope minCheckScope = new())
                {
                    int value = delayed
                                    ? EditorGUI.DelayedIntField(position, minProperty.intValue)
                                    : EditorGUI.IntField(position, minProperty.intValue);

                    if (minCheckScope.changed)
                    {
                        minProperty.intValue = Mathf.Clamp(value, minLimit, maxLimit);
                        maxProperty.intValue = Mathf.Max(maxProperty.intValue, minProperty.intValue);
                    }
                }
            }

            using (new ShowMixedValueScope(minProperty.hasMultipleDifferentValues || maxProperty.hasMultipleDifferentValues))
            {
                position.x += position.width + EditorGUIUtility.standardVerticalSpacing;
                position.width = totalWidth - (fieldWidth * 2) - (EditorGUIUtility.standardVerticalSpacing * 2);

                using (EditorGUI.ChangeCheckScope sliderCheckScope = new())
                {
                    float min = minProperty.hasMultipleDifferentValues ? minLimit : minProperty.intValue;
                    float max = maxProperty.hasMultipleDifferentValues ? maxLimit : maxProperty.intValue;

                    EditorGUI.MinMaxSlider(position, ref min, ref max, minLimit, maxLimit);

                    if (sliderCheckScope.changed)
                    {
                        minProperty.intValue = Mathf.RoundToInt(min);
                        maxProperty.intValue = Mathf.RoundToInt(max);
                    }
                }
            }

            using (new EditorGUI.PropertyScope(position, GUIContent.none, maxProperty))
            {
                position.x += position.width + EditorGUIUtility.standardVerticalSpacing;
                position.width = fieldWidth;

                using (EditorGUI.ChangeCheckScope maxCheckScope = new())
                {
                    int value = delayed
                                    ? EditorGUI.DelayedIntField(position, maxProperty.intValue)
                                    : EditorGUI.IntField(position, maxProperty.intValue);

                    if (maxCheckScope.changed)
                    {
                        maxProperty.intValue = Mathf.Clamp(value, minProperty.intValue, maxLimit);
                    }
                }
            }
        }

        /// <inheritdoc cref="DrawGUI(UnityEngine.Rect,UnityEditor.SerializedProperty,UnityEditor.SerializedProperty,UnityEditor.SerializedProperty,UnityEngine.GUIContent,float,float,bool,bool)"/>
        public static void DrawGUI(Rect position, SerializedProperty minProperty, SerializedProperty maxProperty, float minLimit, float maxLimit, bool delayed)
        {
            position.height = EditorGUIUtility.singleLineHeight;

            const float spacing = 4;
            const float fieldWidth = 50;
            float totalWidth = position.width;

            using (new EditorGUI.PropertyScope(position, GUIContent.none, minProperty))
            {
                position.width = fieldWidth;

                using (EditorGUI.ChangeCheckScope minCheckScope = new())
                {
                    float value = delayed
                                      ? EditorGUI.DelayedFloatField(position, minProperty.floatValue)
                                      : EditorGUI.FloatField(position, minProperty.floatValue);

                    if (minCheckScope.changed)
                    {
                        minProperty.floatValue = Mathf.Clamp(value, minLimit, maxLimit);
                        maxProperty.floatValue = Mathf.Max(maxProperty.floatValue, minProperty.floatValue);
                    }
                }
            }

            using (new ShowMixedValueScope(minProperty.hasMultipleDifferentValues || maxProperty.hasMultipleDifferentValues))
            {
                position.x += position.width + spacing;
                position.width = totalWidth - (fieldWidth * 2) - (spacing * 2);

                using (EditorGUI.ChangeCheckScope sliderCheckScope = new())
                {
                    float min = minProperty.hasMultipleDifferentValues ? minLimit : minProperty.floatValue;
                    float max = maxProperty.hasMultipleDifferentValues ? maxLimit : maxProperty.floatValue;

                    EditorGUI.MinMaxSlider(position, ref min, ref max, minLimit, maxLimit);

                    if (sliderCheckScope.changed)
                    {
                        minProperty.floatValue = Mathf.Round(min * 100) / 100;
                        maxProperty.floatValue = Mathf.Round(max * 100) / 100;
                    }
                }
            }

            using (new EditorGUI.PropertyScope(position, GUIContent.none, maxProperty))
            {
                position.x += position.width + spacing;
                position.width = fieldWidth;

                using (EditorGUI.ChangeCheckScope maxCheckScope = new())
                {
                    float value = delayed
                                      ? EditorGUI.DelayedFloatField(position, maxProperty.floatValue)
                                      : EditorGUI.FloatField(position, maxProperty.floatValue);

                    if (maxCheckScope.changed)
                    {
                        maxProperty.floatValue = Mathf.Clamp(value, minProperty.floatValue, maxLimit);
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

            if (minProperty == null || (minProperty.propertyType != SerializedPropertyType.Integer && minProperty.propertyType != SerializedPropertyType.Float))
            {
                EditorGUI.LabelField(position, label.text, $"{nameof(RangeSliderAttribute)} requires a float or int property named 'x' or '_min'.");

                return;
            }

            SerializedProperty maxProperty = property.FindPropertyRelative("_max") ?? property.FindPropertyRelative("y");

            if (maxProperty == null || (maxProperty.propertyType != SerializedPropertyType.Integer && maxProperty.propertyType != SerializedPropertyType.Float))
            {
                EditorGUI.LabelField(position, label.text, $"{nameof(RangeSliderAttribute)} requires a float or int property named 'y' or '_max'.");

                return;
            }

            if (minProperty.propertyType != maxProperty.propertyType)
            {
                EditorGUI.LabelField(position, label.text, $"{nameof(RangeSliderAttribute)} requires min and max properties to be of same type (float or int).");

                return;
            }

            RangeSliderAttribute rangeSliderAttribute = (RangeSliderAttribute)attribute;
            DrawGUI(position, property, minProperty, maxProperty, label, rangeSliderAttribute.MinLimit, rangeSliderAttribute.MaxLimit, rangeSliderAttribute.RoundToInt, isDelayed);
        }
    }
}
