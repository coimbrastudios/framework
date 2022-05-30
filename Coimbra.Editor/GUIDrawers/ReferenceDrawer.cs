using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Drawer for <see cref="Reference{T}"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(Reference<>))]
    public sealed class ReferenceDrawer : PropertyDrawer
    {
        private const string ValueProperty = "_value";

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property.FindPropertyRelative(ValueProperty), label, true);
        }

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property = property.FindPropertyRelative(ValueProperty);

            using EditorGUI.PropertyScope propertyScope = new EditorGUI.PropertyScope(position, label, property);

            EditorGUI.PropertyField(position, property, propertyScope.content, true);
        }
    }
}
