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
        private static readonly ValueWrapperDrawer Drawer = new("_value");

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Drawer.GetPropertyHeight(property, label);
        }

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Drawer.OnGUI(position, property, label);
        }
    }
}
