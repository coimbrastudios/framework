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

        private static readonly GUIContent ClearLabel = new("Clear");

        private static readonly GUIContent NewLabel = new("New");

        private static readonly TypeDropdownDrawer TypeDropdownDrawer = new();

        /// <summary>
        /// Draws a <see cref="ManagedField{T}"/>. Optionally also allow scene objects to be selected.
        /// </summary>
        public static void DrawGUI(Rect position, SerializedProperty property, GUIContent label, bool allowSceneObjects)
        {
            TooltipAttribute tooltipAttribute = property.GetFieldInfo().GetCustomAttribute<TooltipAttribute>();
            bool enableObjectPicker = GUI.enabled && property.GetFieldInfo().GetCustomAttribute<DisablePickerAttribute>() == null;

            if (tooltipAttribute != null)
            {
                label.tooltip = tooltipAttribute.tooltip;
            }

            using EditorGUI.PropertyScope propertyScope = new(position, label, property);
            SerializedProperty systemObject = property.FindPropertyRelative(SystemObjectProperty);
            SerializedProperty unityObject = property.FindPropertyRelative(UnityObjectProperty);
            Object[] targets = property.serializedObject.targetObjects;

            if (systemObject.GetPropertyPathInfo().HasMultipleDifferentValues(targets))
            {
                DrawMultiEditMessage(position, propertyScope.content, targets, systemObject, unityObject, enableObjectPicker);
            }
            else
            {
                DrawManagedField(position, propertyScope.content, targets, systemObject, unityObject, enableObjectPicker, allowSceneObjects);
            }
        }

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty unityObject = property.FindPropertyRelative(UnityObjectProperty);

            if (unityObject.objectReferenceValue != null)
            {
                return EditorGUI.GetPropertyHeight(unityObject, true);
            }

            SerializedProperty systemObject = property.FindPropertyRelative(SystemObjectProperty);
            Type type = systemObject.GetPropertyType();

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

        private static void DrawInterfaceField(Rect position, GUIContent label, Object[] targets, SerializedProperty systemObject, SerializedProperty unityObject, bool enableObjectPicker, bool allowSceneObjects, Type type)
        {
            string typename = systemObject.managedReferenceFullTypename;

            if (string.IsNullOrWhiteSpace(typename))
            {
                position.height = EditorGUI.GetPropertyHeight(unityObject, true);
                position.width -= MinButtonSize + EditorGUIUtility.standardVerticalSpacing;
                DrawObjectField(position, type, unityObject, label, allowSceneObjects, false);

                position.x = position.xMax + EditorGUIUtility.standardVerticalSpacing;
                position.width = MinButtonSize;
                position.height = EditorGUIUtility.singleLineHeight;

                TypeDropdown.DrawReferenceField(position, type, systemObject, NewLabel, NewUndoKey, delegate(List<Type> list)
                {
                    TypeDropdown.FilterTypes(targets, systemObject.GetScopeInfo(), list);
                });
            }
            else
            {
                Rect valuePosition = position;
                valuePosition.height = EditorGUI.GetPropertyHeight(systemObject, true);
                position.height = EditorGUIUtility.singleLineHeight;
                CoimbraGUIUtility.AdjustPosition(ref position, InspectorArea.Field);

                using (GUIContentPool.Pop(out GUIContent typeLabel))
                {
                    Type selectedType = TypeUtility.GetType(in typename);
                    typeLabel.text = TypeString.Get(selectedType);
                    typeLabel.tooltip = typeLabel.text;
                    EditorGUI.LabelField(position, typeLabel);
                }

                if (enableObjectPicker)
                {
                    position.xMin = position.xMax - MinButtonSize;

                    if (GUI.Button(position, ClearLabel))
                    {
                        Undo.RecordObjects(targets, ClearUndoKey);
                        systemObject.SetValues(null);
                        systemObject.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                        systemObject.serializedObject.UpdateIfRequiredOrScript();
                    }

                    EditorGUI.PropertyField(valuePosition, systemObject, label, true);

                    // HACK: buttons needs to be drawn before to receive the input, but we want to always draw it over the field
                    GUI.Button(position, ClearLabel);
                }
                else
                {
                    EditorGUI.PropertyField(valuePosition, systemObject, label, true);
                }
            }
        }

        private static void DrawManagedField(Rect position, GUIContent label, Object[] targets, SerializedProperty systemObject, SerializedProperty unityObject, bool enableObjectPicker, bool allowSceneObjects)
        {
            Type type = systemObject.GetPropertyType();

            if (typeof(Object).IsAssignableFrom(type))
            {
                position.height = EditorGUI.GetPropertyHeight(unityObject, true);
                DrawObjectField(position, type, unityObject, label, allowSceneObjects, true);
            }
            else if (unityObject.objectReferenceValue != null)
            {
                position.height = EditorGUI.GetPropertyHeight(unityObject, true);

                if (enableObjectPicker)
                {
                    position.width -= MinButtonSize + EditorGUIUtility.standardVerticalSpacing;
                }

                DrawObjectField(position, type, unityObject, label, allowSceneObjects, false);

                if (!enableObjectPicker)
                {
                    return;
                }

                position.x += position.width + EditorGUIUtility.standardVerticalSpacing;
                position.width = MinButtonSize;
                position.height = EditorGUIUtility.singleLineHeight;

                if (!GUI.Button(position, ClearLabel))
                {
                    return;
                }

                Undo.RecordObjects(targets, ClearUndoKey);
                unityObject.SetValues(null);
                unityObject.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                unityObject.serializedObject.UpdateIfRequiredOrScript();
            }
            else if (type.IsInterface)
            {
                DrawInterfaceField(position, label, targets, systemObject, unityObject, enableObjectPicker, allowSceneObjects, type);
            }
            else
            {
                position.height = TypeDropdownDrawer.GetPropertyHeight(unityObject, label);
                TypeDropdownDrawer.OnGUI(position, systemObject, label);
            }
        }

        private static void DrawMultiEditMessage(Rect position, GUIContent label, Object[] targets, SerializedProperty systemObject, SerializedProperty unityObject, bool enableObjectPicker)
        {
            using (GUIContentPool.Pop(out GUIContent value))
            {
                value.text = "Editing multiple different values!";
                value.tooltip = value.text;
                position.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(position, label, value);

                if (!enableObjectPicker)
                {
                    return;
                }

                position.xMin = position.xMax - MinButtonSize;

                if (!GUI.Button(position, ClearLabel))
                {
                    return;
                }

                Undo.RecordObjects(targets, ClearUndoKey);
                systemObject.SetValues(null);
                unityObject.SetValues(null);
                unityObject.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                unityObject.serializedObject.UpdateIfRequiredOrScript();
            }
        }

        private static void DrawObjectField(Rect position, Type type, SerializedProperty property, GUIContent label, bool allowSceneObjects, bool isUnityObjectType)
        {
            using EditorGUI.ChangeCheckScope changeCheckScope = new();

            Object value = EditorGUI.ObjectField(position, label, property.objectReferenceValue, isUnityObjectType ? type : typeof(Object), allowSceneObjects);

            if (!changeCheckScope.changed)
            {
                return;
            }

            if (value != null && !type.IsInstanceOfType(value))
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
