using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Drawer for <see cref="AssetsOnlyAttribute"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(AssetsOnlyAttribute))]
    public sealed class AssetsOnlyDrawer : ValidateDrawer
    {
        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        /// <inheritdoc/>
        protected override void DrawGUI(Rect position, SerializedProperty property, GUIContent label, PropertyPathInfo context, Object[] targets, bool isDelayed)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUI.LabelField(position, label.text, "Use DisallowSceneObjects with Object.");

                return;
            }

            using EditorGUI.PropertyScope propertyScope = new(position, label, property);
            using EditorGUI.ChangeCheckScope changeCheckScope = new();
            Object value = EditorGUI.ObjectField(position, propertyScope.content, property.objectReferenceValue, fieldInfo.FieldType, false);

            if (changeCheckScope.changed)
            {
                property.objectReferenceValue = value;
            }
        }
    }
}
