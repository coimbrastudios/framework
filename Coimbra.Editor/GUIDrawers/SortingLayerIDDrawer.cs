using CoimbraInternal.Editor;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Drawer for <see cref="SortingLayerIDAttribute"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(SortingLayerIDAttribute))]
    public sealed class SortingLayerIDDrawer : ValidateDrawer
    {
        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        /// <inheritdoc/>
        protected override void DrawGUI(Rect position, SerializedProperty property, GUIContent label, PropertyPathInfo context, Object[] targets, bool isDelayed)
        {
            if (property.propertyType != SerializedPropertyType.Integer)
            {
                EditorGUI.LabelField(position, label.text, "Use SortingLayerID with int.");

                return;
            }

            using EditorGUI.PropertyScope propertyScope = new(position, label, property);
            UnityEditorInternals.DrawSortingLayerField(position, propertyScope.content, property);
        }
    }
}
