using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Coimbra.Editor
{
    [CustomPropertyDrawer(typeof(InterfaceField<>), true)]
    public sealed class InterfaceFieldDrawer : PropertyDrawer
    {
        private const string SystemObjectSerializedProperty = "_systemObject";
        private const string UnityObjectSerializedProperty = "_unityObject";

        private static readonly GUIContent HiddenLabel = new GUIContent(" ");

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty systemObject = property.FindPropertyRelative(SystemObjectSerializedProperty);
            float systemObjectHeight = EditorGUI.GetPropertyHeight(systemObject, true);
            SerializedProperty unityObject = property.FindPropertyRelative(UnityObjectSerializedProperty);
            float unityObjectHeight = EditorGUI.GetPropertyHeight(unityObject, true);

            return Mathf.Max(systemObjectHeight, unityObjectHeight);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            OnGUI(position, property, label, true);
        }

        public void OnGUI(Rect position, SerializedProperty property, GUIContent label, bool allowSceneObjects)
        {
            object[] tooltipAttributeArray = fieldInfo.GetCustomAttributes(typeof(TooltipAttribute), true);

            if (tooltipAttributeArray.Length > 0)
            {
                TooltipAttribute tooltipAttribute = (TooltipAttribute)tooltipAttributeArray[0];
                label.tooltip = tooltipAttribute.tooltip;
            }

            Type baseType = fieldInfo.FieldType.IsArray ? fieldInfo.FieldType.GetElementType() : fieldInfo.FieldType;
            Assert.IsNotNull(baseType);

#if !UNITY_2020_1_OR_NEWER
            while (baseType.BaseType != null && baseType.BaseType != typeof(object) && baseType.BaseType != typeof(ValueType))
            {
                baseType = baseType.BaseType;
            }
#endif

            Type interfaceType = baseType.GenericTypeArguments[0];
            string suffix = $"* {interfaceType.FullName}";
            string tooltip = string.IsNullOrEmpty(label.tooltip) ? suffix : $"{label.tooltip}{Environment.NewLine}{suffix}";
            GUIContent labelWithTooltip = new GUIContent($"{label.text}*", label.image, tooltip);
            SerializedProperty systemObject = property.FindPropertyRelative(SystemObjectSerializedProperty);
            position.height = EditorGUI.GetPropertyHeight(systemObject, true);
            EditorGUI.PropertyField(position, systemObject, HiddenLabel, true);

            SerializedProperty unityObject = property.FindPropertyRelative(UnityObjectSerializedProperty);

            using (EditorGUI.PropertyScope propertyScope = new EditorGUI.PropertyScope(position, labelWithTooltip, unityObject))
            {
                using (EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    position.height = EditorGUI.GetPropertyHeight(unityObject, true);

                    UnityEngine.Object value = EditorGUI.ObjectField(position, propertyScope.content, unityObject.objectReferenceValue, typeof(UnityEngine.Object), allowSceneObjects);

                    if (changeCheckScope.changed)
                    {
                        if (value != null && interfaceType.IsInstanceOfType(value) == false)
                        {
                            value = value is GameObject gameObject ? gameObject.GetComponent(interfaceType) : null;

                            if (value == null)
                            {
                                Debug.LogError($"The object or one of its components doesn't implement {interfaceType.FullName}!", value);
                            }
                        }

                        unityObject.objectReferenceValue = value;
                        systemObject.managedReferenceValue = null;
                    }
                }
            }
        }
    }
}
