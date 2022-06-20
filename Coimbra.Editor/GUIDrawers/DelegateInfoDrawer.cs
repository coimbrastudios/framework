using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Drawer for <see cref="DelegateInfo"/>
    /// </summary>
    [CustomPropertyDrawer(typeof(DelegateInfo))]
    public sealed class DelegateInfoDrawer : PropertyDrawer
    {
        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.DisabledScope(true))
            {
                SerializedProperty target = property.FindPropertyRelative("_target");
                label.text = target.hasMultipleDifferentValues ? "-" : target.stringValue;
                position = EditorGUI.PrefixLabel(position, label);

                SerializedProperty method = property.FindPropertyRelative("_method");
                EditorGUI.TextField(position, method.hasMultipleDifferentValues ? "-" : method.stringValue);
            }
        }
    }
}
