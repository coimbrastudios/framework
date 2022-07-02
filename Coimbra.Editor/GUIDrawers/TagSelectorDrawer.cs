using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Drawer for <see cref="TagSelectorAttribute"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(TagSelectorAttribute))]
    public sealed class TagSelectorDrawer : ValidateDrawer
    {
        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        /// <inheritdoc/>
        protected override void DrawGUI(Rect position, SerializedProperty property, GUIContent label, PropertyPathInfo context, Object[] targets, bool isDelayed)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Use TagSelector with string.");

                return;
            }

            string[] tags = InternalEditorUtility.tags;

            object setValues(PropertyPathInfo sender, Object target)
            {
                sender.TryGetValue(target, out string value);

                if (string.IsNullOrEmpty(value))
                {
                    return tags[0];
                }

                for (int i = 0; i < tags.Length; i++)
                {
                    if (value == tags[i])
                    {
                        return value;
                    }
                }

                return tags[0];
            }

            context.SetValues(targets, true, setValues);

            using EditorGUI.PropertyScope propertyScope = new(position, label, property);
            using EditorGUI.ChangeCheckScope changeCheckScope = new();
            {
                string value = EditorGUI.TagField(position, propertyScope.content, property.hasMultipleDifferentValues ? "-" : property.stringValue);

                if (changeCheckScope.changed)
                {
                    property.stringValue = value;
                }
            }
        }
    }
}
