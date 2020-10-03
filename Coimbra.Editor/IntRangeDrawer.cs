using UnityEditor;
using UnityEngine;

namespace Coimbra
{
    [CustomPropertyDrawer(typeof(IntRange))]
    public sealed class IntRangeDrawer : PropertyDrawer
    {
        private const string MaxSerializedProperty = "_max";
        private const string MinSerializedProperty = "_min";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            OnGUI(position, property, label, false);
        }

        public void OnGUI(Rect position, SerializedProperty property, GUIContent label, bool delayed)
        {
            using (EditorGUI.PropertyScope propertyScope = new EditorGUI.PropertyScope(position, label, property))
            {
                position.height = EditorGUIUtility.singleLineHeight;

                Rect labelPosition = position;
                labelPosition.width = EditorGUIUtility.labelWidth;
                EditorGUI.LabelField(labelPosition, propertyScope.content);

                using (new EditorGUI.IndentLevelScope(-EditorGUI.indentLevel))
                {
                    position.x += labelPosition.width;
                    position.width -= labelPosition.width;

                    SerializedProperty minProperty = property.FindPropertyRelative(MinSerializedProperty);
                    SerializedProperty maxProperty = property.FindPropertyRelative(MaxSerializedProperty);

                    DrawGUI(position, minProperty, maxProperty, delayed);
                }
            }
        }

        private void DrawGUI(Rect position, SerializedProperty minProperty, SerializedProperty maxProperty, bool delayed)
        {
            position.height = EditorGUIUtility.singleLineHeight;

            const float spacing = 2;
            const float labelWidth = 28;
            float totalWidth = position.width;
            float fieldWith = totalWidth / 2 - spacing / 2 - labelWidth;

            using (EditorGUI.PropertyScope propertyScope = new EditorGUI.PropertyScope(position, new GUIContent("Min"), minProperty))
            {
                position.width = labelWidth;
                EditorGUI.LabelField(position, propertyScope.content);

                position.x += position.width;
                position.width = fieldWith;

                using (EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    int value = delayed
                                    ? EditorGUI.DelayedIntField(position, minProperty.intValue)
                                    : EditorGUI.IntField(position, minProperty.intValue);

                    if (changeCheckScope.changed)
                    {
                        minProperty.intValue = value;
                        maxProperty.intValue = Mathf.Max(value, maxProperty.intValue);
                    }
                }
            }

            using (EditorGUI.PropertyScope propertyScope = new EditorGUI.PropertyScope(position, new GUIContent("Max"), maxProperty))
            {
                position.x += position.width + spacing;
                position.width = labelWidth;
                EditorGUI.LabelField(position, propertyScope.content);

                position.x += position.width;
                position.width = fieldWith;

                using (EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    int value = delayed
                                    ? EditorGUI.DelayedIntField(position, maxProperty.intValue)
                                    : EditorGUI.IntField(position, maxProperty.intValue);

                    if (changeCheckScope.changed)
                    {
                        maxProperty.intValue = Mathf.Max(value, minProperty.intValue);
                    }
                }
            }
        }
    }
}
