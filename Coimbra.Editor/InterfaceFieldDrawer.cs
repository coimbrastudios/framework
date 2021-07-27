using System;
using System.Reflection;
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
        private static readonly GUIContent ClearLabel = new GUIContent("Clear");
        private static readonly GUIContent EmptyLabel = new GUIContent(" ");
        private static readonly GUIContent NewLabel = new GUIContent("New");

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty unityObject = property.FindPropertyRelative(UnityObjectSerializedProperty);

            if (unityObject.objectReferenceValue != null)
            {
                return EditorGUI.GetPropertyHeight(unityObject, true);
            }

            SerializedProperty systemObject = property.FindPropertyRelative(SystemObjectSerializedProperty);

            return string.IsNullOrWhiteSpace(systemObject.managedReferenceFullTypename) ? EditorGUIUtility.singleLineHeight : EditorGUI.GetPropertyHeight(systemObject, true);
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

            Type interfaceType = baseType.GenericTypeArguments[0];
            string suffix = $"* {interfaceType.FullName}";
            string tooltip = string.IsNullOrEmpty(label.tooltip) ? suffix : $"{label.tooltip}{Environment.NewLine}{suffix}";
            GUIContent labelWithTooltip = new GUIContent($"{label.text}*", label.image, tooltip);
            SerializedProperty systemObject = property.FindPropertyRelative(SystemObjectSerializedProperty);
            SerializedProperty unityObject = property.FindPropertyRelative(UnityObjectSerializedProperty);

            using EditorGUI.PropertyScope propertyScope = new EditorGUI.PropertyScope(position, labelWithTooltip, unityObject);

            position.height = EditorGUI.GetPropertyHeight(unityObject, true);

            if (unityObject.objectReferenceValue != null)
            {
                float buttonWidth = EditorStyles.miniPullDown.CalcSize(ClearLabel).x;
                Rect unityFieldPosition = position;
                unityFieldPosition.xMax -= buttonWidth + EditorGUIUtility.standardVerticalSpacing;
                DrawUnityField(unityFieldPosition, interfaceType, systemObject, unityObject, propertyScope.content, allowSceneObjects);

                Rect buttonPosition = position;
                buttonPosition.xMin = unityFieldPosition.xMax + EditorGUIUtility.standardVerticalSpacing;

                if (GUI.Button(buttonPosition, ClearLabel, EditorStyles.miniButton))
                {
                    systemObject.serializedObject.Update();
                    unityObject.objectReferenceValue = null;
                    systemObject.serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                string typename = systemObject.managedReferenceFullTypename;

                if (string.IsNullOrWhiteSpace(typename))
                {
                    float dropdownWidth = EditorStyles.miniPullDown.CalcSize(NewLabel).x;
                    Rect unityFieldPosition = position;
                    unityFieldPosition.xMax -= dropdownWidth + EditorGUIUtility.standardVerticalSpacing;
                    DrawUnityField(unityFieldPosition, interfaceType, systemObject, unityObject, propertyScope.content, allowSceneObjects);

                    Rect systemDropdownPosition = position;
                    systemDropdownPosition.xMin = unityFieldPosition.xMax + EditorGUIUtility.standardVerticalSpacing;
                    DrawSystemDropdown(systemDropdownPosition, interfaceType, systemObject);
                }
                else
                {
                    float buttonWidth = EditorStyles.miniButton.CalcSize(ClearLabel).x;
                    string separator = typename.Contains(".") ? "." : " ";
                    GUIContent value = new GUIContent(typename.Substring(typename.LastIndexOf(separator, StringComparison.Ordinal) + 1));
                    Rect labelPosition = position;
                    labelPosition.xMax -= buttonWidth + EditorGUIUtility.standardVerticalSpacing;
                    EditorGUI.LabelField(labelPosition, EmptyLabel, value);

                    Rect buttonPosition = position;
                    buttonPosition.xMin = labelPosition.xMax + EditorGUIUtility.standardVerticalSpacing;

                    if (GUI.Button(buttonPosition, ClearLabel, EditorStyles.miniButton))
                    {
                        systemObject.serializedObject.Update();
                        systemObject.managedReferenceValue = null;
                        systemObject.serializedObject.ApplyModifiedProperties();
                    }

                    Rect valuePosition = position;
                    valuePosition.height = EditorGUI.GetPropertyHeight(systemObject, true);
                    EditorGUI.PropertyField(valuePosition, systemObject, propertyScope.content, true);
                }
            }
        }

        private static void DrawSystemDropdown(Rect position, Type interfaceType, SerializedProperty systemObject)
        {
            if (!EditorGUI.DropdownButton(position, NewLabel, FocusType.Passive))
            {
                return;
            }

            void handleItemClicked(object parameter)
            {
                systemObject.serializedObject.Update();
                systemObject.managedReferenceValue = Activator.CreateInstance((Type)parameter);
                systemObject.isExpanded = true;
                systemObject.serializedObject.ApplyModifiedProperties();
            }

            GenericMenu menu = new GenericMenu
            {
                allowDuplicateNames = false,
            };

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsAbstract || !interfaceType.IsAssignableFrom(type) || type.IsSubclassOf(typeof(UnityEngine.Object)))
                    {
                        continue;
                    }

                    if (type.IsValueType || type.GetConstructor(Type.EmptyTypes) != null)
                    {
                        menu.AddItem(new GUIContent(type.FullName), false, handleItemClicked, type);
                    }
                }
            }

            if (menu.GetItemCount() == 0)
            {
                Debug.LogWarning($"No type that implements {interfaceType} was found! The type also needs to be a non-abstract type with parameterless constructor and can't be a subclass of UnityEngine.Object.");
            }
            else
            {
                menu.DropDown(position);
            }
        }

        private static void DrawUnityField(Rect position, Type interfaceType, SerializedProperty systemObject, SerializedProperty unityObject, GUIContent label, bool allowSceneObjects)
        {
            using EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope();

            UnityEngine.Object value = EditorGUI.ObjectField(position, label, unityObject.objectReferenceValue, typeof(UnityEngine.Object), allowSceneObjects);

            if (!changeCheckScope.changed)
            {
                return;
            }

            if (value != null && interfaceType.IsInstanceOfType(value) == false)
            {
                value = value is GameObject gameObject ? gameObject.GetComponent(interfaceType) : null;

                if (value == null)
                {
                    Debug.LogError($"Neither the object or one of its components implement {interfaceType.FullName}!", value);
                }
            }

            unityObject.objectReferenceValue = value;
            systemObject.managedReferenceValue = null;
        }
    }
}
