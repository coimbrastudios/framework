using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Coimbra.Editor
{
    /// <summary>
    /// Drawer for <see cref="ManagedField{T}"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(ManagedField<>))]
    public sealed class ManagedFieldDrawer : PropertyDrawer
    {
        private const int MinButtonSize = 50;

        private const string ClearUndoKey = "Clear Field Value";

        private const string NewUndoKey = "New Field Value";

        private const string SystemObjectProperty = "_systemObject";

        private const string UnityObjectProperty = "_unityObject";

        private static readonly GUIContent ClearLabel = new GUIContent("Clear");

        private static readonly GUIContent NewLabel = new GUIContent("New");

        private static readonly TypeDropdownDrawer TypeDropdownDrawer = new TypeDropdownDrawer();

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty unityObject = property.FindPropertyRelative(UnityObjectProperty);

            if (unityObject.objectReferenceValue != null)
            {
                return EditorGUI.GetPropertyHeight(unityObject, true);
            }

            SerializedProperty systemObject = property.FindPropertyRelative(SystemObjectProperty);
            Type type = systemObject.GetFieldInfo().FieldType;

            if (typeof(Object).IsAssignableFrom(type) || systemObject.hasMultipleDifferentValues)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            return string.IsNullOrWhiteSpace(systemObject.managedReferenceFullTypename) ? EditorGUIUtility.singleLineHeight : EditorGUI.GetPropertyHeight(systemObject, true);
        }

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawGUI(position, property, label, true);
        }

        /// <summary>
        /// Draws a <see cref="ManagedField{T}"/>. Optionally also allow scene objects to be selected.
        /// </summary>
        public void DrawGUI(Rect position, SerializedProperty property, GUIContent label, bool allowSceneObjects)
        {
            TooltipAttribute tooltipAttribute = fieldInfo.GetCustomAttribute<TooltipAttribute>();

            if (tooltipAttribute != null)
            {
                label.tooltip = tooltipAttribute.tooltip;
            }

            using EditorGUI.PropertyScope propertyScope = new EditorGUI.PropertyScope(position, label, property);
            SerializedProperty systemObject = property.FindPropertyRelative(SystemObjectProperty);
            SerializedProperty unityObject = property.FindPropertyRelative(UnityObjectProperty);

            if (systemObject.GetPropertyPathInfo().HasMultipleDifferentValues(property.serializedObject.targetObjects))
            {
                using (GUIContentPool.Pop(out GUIContent value))
                {
                    value.text = "Editing multiple different values!";
                    value.tooltip = value.text;
                    position.height = EditorGUIUtility.singleLineHeight;
                    EditorGUI.LabelField(position, propertyScope.content, value);

                    position.xMin = position.xMax - MinButtonSize;

                    if (GUI.Button(position, ClearLabel))
                    {
                        Undo.RecordObjects(property.serializedObject.targetObjects, ClearUndoKey);
                        systemObject.SetValues(null);
                        unityObject.SetValues(null);
                        unityObject.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                        unityObject.serializedObject.UpdateIfRequiredOrScript();
                    }

                    return;
                }
            }

            Type type = systemObject.GetFieldInfo().FieldType;

            if (typeof(Object).IsAssignableFrom(type))
            {
                position.height = EditorGUI.GetPropertyHeight(unityObject, true);
                DrawObjectField(position, type, unityObject, propertyScope.content, allowSceneObjects, true);
            }
            else if (unityObject.objectReferenceValue != null)
            {
                position.height = EditorGUI.GetPropertyHeight(unityObject, true);
                position.width -= MinButtonSize + EditorGUIUtility.standardVerticalSpacing;
                DrawObjectField(position, type, unityObject, propertyScope.content, allowSceneObjects, false);

                position.x += position.width + EditorGUIUtility.standardVerticalSpacing;
                position.width = MinButtonSize;
                position.height = EditorGUIUtility.singleLineHeight;

                if (GUI.Button(position, ClearLabel))
                {
                    Undo.RecordObjects(property.serializedObject.targetObjects, ClearUndoKey);
                    unityObject.SetValues(null);
                    unityObject.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                    unityObject.serializedObject.UpdateIfRequiredOrScript();
                }
            }
            else if (type.IsInterface)
            {
                string typename = systemObject.managedReferenceFullTypename;

                if (string.IsNullOrWhiteSpace(typename))
                {
                    position.height = EditorGUI.GetPropertyHeight(unityObject, true);
                    position.width -= MinButtonSize + EditorGUIUtility.standardVerticalSpacing;
                    DrawObjectField(position, type, unityObject, propertyScope.content, allowSceneObjects, false);

                    position.x = position.xMax + EditorGUIUtility.standardVerticalSpacing;
                    position.width = MinButtonSize;
                    position.height = EditorGUIUtility.singleLineHeight;

                    TypeDropdown.Draw(position, type, systemObject, NewLabel, NewUndoKey, delegate(List<Type> list)
                    {
                        TypeDropdown.FilterTypes(property.serializedObject.targetObjects, systemObject.GetScope(), list);
                    });
                }
                else
                {
                    Rect valuePosition = position;
                    valuePosition.height = EditorGUI.GetPropertyHeight(systemObject, true);
                    position.xMin += EditorGUIUtility.labelWidth;
                    position.height = EditorGUIUtility.singleLineHeight;

                    using (GUIContentPool.Pop(out GUIContent typeLabel))
                    {
                        Type selectedType = TypeUtility.GetType(in typename);
                        typeLabel.text = TypeString.Get(selectedType);
                        typeLabel.tooltip = typeLabel.text;
                        EditorGUI.LabelField(position, typeLabel);
                    }

                    position.xMin = position.xMax - MinButtonSize;

                    if (GUI.Button(position, ClearLabel))
                    {
                        Undo.RecordObjects(property.serializedObject.targetObjects, ClearUndoKey);
                        systemObject.SetValues(null);
                        systemObject.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                        systemObject.serializedObject.UpdateIfRequiredOrScript();
                    }

                    EditorGUI.PropertyField(valuePosition, systemObject, propertyScope.content, true);

                    // HACK: buttons needs to be drawn before to receive the input, but we want to always draw it over the field
                    GUI.Button(position, ClearLabel);
                }
            }
            else
            {
                TypeDropdownDrawer.OnGUI(position, systemObject, propertyScope.content);
            }
        }

        private static void DrawObjectField(Rect position, Type type, SerializedProperty property, GUIContent label, bool allowSceneObjects, bool isUnityObjectType)
        {
            using EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope();

            Object value = EditorGUI.ObjectField(position, label, property.objectReferenceValue, isUnityObjectType ? type : typeof(Object), allowSceneObjects);

            if (!changeCheckScope.changed)
            {
                return;
            }

            if (value != null && type.IsInstanceOfType(value) == false)
            {
                value = value is GameObject gameObject ? gameObject.GetComponent(type) : null;

                if (value == null)
                {
                    Debug.LogError($"Neither the object or one of its components implement {TypeString.Get(type)}!", value);
                }
            }

            property.objectReferenceValue = value;
        }
    }
}
