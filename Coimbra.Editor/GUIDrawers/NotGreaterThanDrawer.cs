using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Drawer for <see cref="NotGreaterThanAttribute"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(NotGreaterThanAttribute))]
    public sealed class NotGreaterThanDrawer : ValidateDrawer
    {
        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        /// <inheritdoc/>
        protected override void DrawGUI(Rect position, SerializedProperty property, GUIContent label, PropertyPathInfo context, Object[] targets, bool isDelayed)
        {
            NotGreaterThanAttribute notGreaterThanAttribute = (NotGreaterThanAttribute)attribute;

            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                {
                    int maxValue = Mathf.FloorToInt(notGreaterThanAttribute.Value);

                    int setValue(PropertyPathInfo sender, Object target)
                    {
                        return Mathf.Min(sender.GetValue<int>(target), maxValue);
                    }

                    context.SetValues(targets, true, setValue);

                    using EditorGUI.PropertyScope propertyScope = new(position, label, property);
                    using EditorGUI.ChangeCheckScope changeCheckScope = new();

                    int value = notGreaterThanAttribute.Delayed
                                    ? EditorGUI.DelayedIntField(position, propertyScope.content, property.intValue)
                                    : EditorGUI.IntField(position, propertyScope.content, property.intValue);

                    if (changeCheckScope.changed)
                    {
                        property.intValue = Mathf.Min(value, maxValue);
                    }

                    break;
                }

                case SerializedPropertyType.Float:
                {
                    float setValue(PropertyPathInfo sender, Object target)
                    {
                        return Mathf.Min(sender.GetValue<float>(target), notGreaterThanAttribute.Value);
                    }

                    context.SetValues(targets, true, setValue);

                    using EditorGUI.PropertyScope propertyScope = new(position, label, property);
                    using EditorGUI.ChangeCheckScope changeCheckScope = new();

                    float value = notGreaterThanAttribute.Delayed
                                      ? EditorGUI.DelayedFloatField(position, propertyScope.content, property.floatValue)
                                      : EditorGUI.FloatField(position, propertyScope.content, property.floatValue);

                    if (changeCheckScope.changed)
                    {
                        property.floatValue = Mathf.Min(value, notGreaterThanAttribute.Value);
                    }

                    break;
                }

                default:
                {
                    EditorGUI.LabelField(position, label.text, "Use Max with int or float.");

                    break;
                }
            }
        }
    }
}
