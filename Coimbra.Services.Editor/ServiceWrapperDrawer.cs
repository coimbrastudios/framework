using UnityEditor;
using UnityEngine;

namespace Coimbra.Services.Editor
{
    [CustomPropertyDrawer(typeof(ServiceWrapper))]
    internal sealed class ServiceWrapperDrawer : PropertyDrawer
    {
        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty type = property.FindPropertyRelative(nameof(ServiceWrapper.Type));

            if (!type.isExpanded)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            SerializedProperty value = property.FindPropertyRelative(nameof(ServiceWrapper.Value));
            SerializedProperty factory = property.FindPropertyRelative(nameof(ServiceWrapper.Factory));

            return EditorGUIUtility.singleLineHeight + EditorGUI.GetPropertyHeight(value) + EditorGUI.GetPropertyHeight(factory) + EditorGUIUtility.standardVerticalSpacing * 2;
        }

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty type = property.FindPropertyRelative(nameof(ServiceWrapper.Type));
            position.height = EditorGUIUtility.singleLineHeight;

            type.isExpanded = EditorGUI.Foldout(position, type.isExpanded, type.stringValue, true);

            if (!type.isExpanded)
            {
                return;
            }

            position.y += position.height + EditorGUIUtility.standardVerticalSpacing;

            SerializedProperty factory = property.FindPropertyRelative(nameof(ServiceWrapper.Factory));
            position.height = EditorGUI.GetPropertyHeight(factory);
            label.text = factory.displayName;

            Rect factoryPosition = EditorGUI.PrefixLabel(position, label);

            EditorGUI.SelectableLabel(factoryPosition, factory.stringValue);

            using (new EditorGUI.DisabledScope(true))
            {
                position.y += position.height + EditorGUIUtility.standardVerticalSpacing;

                SerializedProperty value = property.FindPropertyRelative(nameof(ServiceWrapper.Value));
                position.height = EditorGUI.GetPropertyHeight(value);
                EditorGUI.PropertyField(position, value);
            }
        }
    }
}
