using UnityEditor;
using UnityEngine;

namespace Coimbra.Services.Events.Editor
{
    /// <summary>
    /// Drawer for <see cref="EventSystem"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(EventSystem))]
    public sealed class EventSystemDrawer : PropertyDrawer
    {
        private const string ListProperty = "_list";

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            float height = EditorGUIUtility.singleLineHeight;
            SerializedProperty listProperty = property.FindPropertyRelative(ListProperty);
            int listSize = listProperty.arraySize;

            for (int i = 0; i < listSize; i++)
            {
                height += EditorGUI.GetPropertyHeight(listProperty.GetArrayElementAtIndex(i)) + EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight * 0.4f;
            }

            return height;
        }

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;

            if (!EditorGUI.PropertyField(position, property, label, false))
            {
                return;
            }

            using (new EditorGUI.IndentLevelScope())
            {
                SerializedProperty listProperty = property.FindPropertyRelative(ListProperty);
                int listSize = listProperty.arraySize;

                for (int i = 0; i < listSize; i++)
                {
                    SerializedProperty elementProperty = listProperty.GetArrayElementAtIndex(i);
                    position.y += position.height + EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight * 0.2f;
                    position.height = EditorGUI.GetPropertyHeight(elementProperty);
                    EditorGUI.PropertyField(position, elementProperty);

                    position.y += EditorGUIUtility.singleLineHeight * 0.2f;
                }
            }
        }
    }
}
