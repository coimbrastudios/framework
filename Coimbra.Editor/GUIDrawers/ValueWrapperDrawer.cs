using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Drawer for any class that just wraps a value.
    /// </summary>
    /// <seealso cref="Delayed{T}"/>
    [CustomPropertyDrawer(typeof(Delayed<>))]
    public sealed class ValueWrapperDrawer : PropertyDrawer
    {
        private readonly string _valueProperty;

        public ValueWrapperDrawer(string valueProperty = "Value")
        {
            _valueProperty = valueProperty;
        }

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property.FindPropertyRelative(_valueProperty), label, true);
        }

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property = property.FindPropertyRelative(_valueProperty);

            using EditorGUI.PropertyScope propertyScope = new(position, label, property);

            EditorGUI.PropertyField(position, property, propertyScope.content, true);
        }
    }
}
