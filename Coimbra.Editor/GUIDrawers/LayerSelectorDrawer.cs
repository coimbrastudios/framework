using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Drawer for <see cref="LayerSelectorAttribute"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(LayerSelectorAttribute))]
    public sealed class LayerSelectorDrawer : ValidateDrawer
    {
        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        /// <inheritdoc/>
        protected override void DrawGUI(Rect position, SerializedProperty property, GUIContent label, PropertyPathInfo context, Object[] targets, bool isDelayed)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                {
                    DrawIntField(position, property, label, context, targets);

                    break;
                }

                case SerializedPropertyType.String:
                {
                    DrawStringField(position, property, label, context, targets);

                    break;
                }

                default:
                {
                    EditorGUI.LabelField(position, label.text, "Use LayerSelector with int or string.");

                    break;
                }
            }
        }

        private static void DrawIntField(Rect position, SerializedProperty property, GUIContent label, PropertyPathInfo context, Object[] targets)
        {
            string[] layers = InternalEditorUtility.layers;

            int setValue(PropertyPathInfo sender, Object target)
            {
                sender.TryGetValue(target, out int value);

                if (string.IsNullOrEmpty(LayerMask.LayerToName(value)))
                {
                    return LayerMask.NameToLayer(layers[0]);
                }

                for (int i = 0; i < layers.Length; i++)
                {
                    if (LayerMask.LayerToName(value) == layers[i])
                    {
                        return value;
                    }
                }

                return LayerMask.NameToLayer(layers[0]);
            }

            context.SetValues(targets, true, setValue);

            using EditorGUI.PropertyScope propertyScope = new(position, label, property);
            using EditorGUI.ChangeCheckScope changeCheckScope = new();
            {
                int value = EditorGUI.LayerField(position, propertyScope.content, property.intValue);

                if (changeCheckScope.changed)
                {
                    property.intValue = value;
                }
            }
        }

        private static void DrawStringField(Rect position, SerializedProperty property, GUIContent label, PropertyPathInfo context, Object[] targets)
        {
            string[] layers = InternalEditorUtility.layers;

            string setValue(PropertyPathInfo sender, Object target)
            {
                sender.TryGetValue(target, out string value);

                if (string.IsNullOrEmpty(value))
                {
                    return layers[0];
                }

                for (int i = 0; i < layers.Length; i++)
                {
                    if (value == layers[i])
                    {
                        return value;
                    }
                }

                return layers[0];
            }

            context.SetValues(targets, true, setValue);

            using EditorGUI.PropertyScope propertyScope = new(position, label, property);
            using EditorGUI.ChangeCheckScope changeCheckScope = new();
            {
                int value = EditorGUI.LayerField(position, propertyScope.content, LayerMask.NameToLayer(property.stringValue));

                if (changeCheckScope.changed)
                {
                    property.stringValue = LayerMask.LayerToName(value);
                }
            }
        }
    }
}
