using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Drawer for <see cref="SelectableLabelAttribute"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(SelectableLabelAttribute))]
    public sealed class SelectableLabelDrawer : PropertyDrawer
    {
        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using EditorGUI.PropertyScope propertyScope = new(position, label, property);
            position = EditorGUI.PrefixLabel(position, propertyScope.content);

            using (new ResetIndentLevelScope())
            {
                EditorGUI.SelectableLabel(position, property.stringValue);
            }
        }
    }
}
