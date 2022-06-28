using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Drawer for <see cref="DelegateListener"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(DelegateListener))]
    public sealed class DelegateListenerDrawer : PropertyDrawer
    {
        public static void DrawGUI(Rect position, in string target, in string method, bool isStatic)
        {
            if (isStatic)
            {
                EditorGUI.TextField(position, method);
            }
            else
            {
                using (GUIContentPool.Pop(out GUIContent temp))
                {
                    temp.text = target;

                    using (new EditorGUI.DisabledScope(true))
                    {
                        position = EditorGUI.PrefixLabel(position, temp);
                        EditorGUI.TextField(position, method);
                    }
                }
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
                SerializedProperty isStatic = property.FindPropertyRelative("_isStatic");

                if (isStatic.hasMultipleDifferentValues)
                {
                    EditorGUI.LabelField(position, "-");
                }
                else if (isStatic.boolValue)
                {
                    SerializedProperty method = property.FindPropertyRelative("_method");
                    DrawGUI(position, null, method.hasMultipleDifferentValues ? "-" : method.stringValue, true);
                }
                else
                {
                    SerializedProperty target = property.FindPropertyRelative("_target");
                    label.text = target.hasMultipleDifferentValues ? "-" : target.stringValue;
                    SerializedProperty method = property.FindPropertyRelative("_method");
                    DrawGUI(position, label.text, method.hasMultipleDifferentValues ? "-" : method.stringValue, false);
                }
            }
        }
    }
}
