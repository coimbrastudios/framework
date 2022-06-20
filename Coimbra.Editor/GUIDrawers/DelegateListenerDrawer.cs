using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Drawer for <see cref="DelegateListener"/>
    /// </summary>
    [CustomPropertyDrawer(typeof(DelegateListener))]
    public sealed class DelegateListenerDrawer : PropertyDrawer
    {
        public static void DrawGUI(Rect position, GUIContent target, string method)
        {
            using (new EditorGUI.DisabledScope(true))
            {
                position = EditorGUI.PrefixLabel(position, target);
                EditorGUI.TextField(position, method);
            }
        }

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
                SerializedProperty method = property.FindPropertyRelative("_method");
                DrawGUI(position, label, method.hasMultipleDifferentValues ? "-" : method.stringValue);
            }
        }
    }
}
