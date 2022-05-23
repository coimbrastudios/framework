using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
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

        private const int MinDropdownLineCount = 10;

        private const float MinDropdownWidth = 400;

        private const string ClearUndoKey = "Clear Field Value";

        private const string NewUndoKey = "New Field Value";

        private const string SystemObjectProperty = "_systemObject";

        private const string UnityObjectProperty = "_unityObject";

        private static readonly GUIContent ClearLabel = new GUIContent("Clear");

        private static readonly GUIContent NewLabel = new GUIContent("New");

        private static SerializedProperty _current;

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
            else
            {
                string typename = systemObject.managedReferenceFullTypename;

                if (string.IsNullOrWhiteSpace(typename))
                {
                    if (type.IsInterface)
                    {
                        position.height = EditorGUI.GetPropertyHeight(unityObject, true);
                        position.width -= MinButtonSize + EditorGUIUtility.standardVerticalSpacing;
                        DrawObjectField(position, type, unityObject, propertyScope.content, allowSceneObjects, false);

                        position.x += position.width + EditorGUIUtility.standardVerticalSpacing;
                        position.width = MinButtonSize;
                        position.height = EditorGUIUtility.singleLineHeight;
                        DrawTypeDropdown(position, type, systemObject);
                    }
                    else
                    {
                        position.height = EditorGUI.GetPropertyHeight(systemObject, true);
                        EditorGUI.PropertyField(position, systemObject, propertyScope.content, true);

                        position.xMin += EditorGUIUtility.labelWidth;
                        position.height = EditorGUIUtility.singleLineHeight;
                        DrawTypeDropdown(position, type, systemObject);
                    }
                }
                else
                {
                    Rect valuePosition = position;
                    valuePosition.height = EditorGUI.GetPropertyHeight(systemObject, true);
                    position.xMin += EditorGUIUtility.labelWidth;
                    position.height = EditorGUIUtility.singleLineHeight;

                    using (GUIContentPool.Pop(out GUIContent value))
                    {
                        value.text = TypeString.Get(GetType(typename));
                        value.tooltip = value.text;
                        EditorGUI.LabelField(position, value);
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

        private static void DrawTypeDropdown(Rect position, Type type, SerializedProperty property)
        {
            if (!EditorGUI.DropdownButton(position, NewLabel, FocusType.Keyboard))
            {
                return;
            }

            using (ListPool.Pop(out List<Type> types))
            {
                foreach (Type derivedType in TypeCache.GetTypesDerivedFrom(type))
                {
                    if (derivedType.IsAbstract
                     || derivedType.IsGenericType
                     || typeof(Object).IsAssignableFrom(derivedType)
                     || derivedType.IsDefined(typeof(CompilerGeneratedAttribute))
                     || !derivedType.IsDefined(typeof(SerializableAttribute)))
                    {
                        continue;
                    }

                    const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

                    if (!derivedType.IsValueType && derivedType.GetConstructor(bindingFlags, null, Type.EmptyTypes, null) == null)
                    {
                        continue;
                    }

                    types.Add(derivedType);
                }

                FilterTypes(property, types);

                _current = property;

                static void handleItemSelected(TypeDropdownItem item)
                {
                    Undo.RecordObjects(_current.serializedObject.targetObjects, NewUndoKey);

                    if (item.Type == null)
                    {
                        _current.SetValues(null);
                    }
                    else
                    {
                        _current.SetValues(true, delegate
                        {
                            return Activator.CreateInstance(item.Type);
                        });
                    }

                    _current.isExpanded = item.Type != null;
                    _current.serializedObject.ApplyModifiedProperties();
                    _current.serializedObject.Update();
                }

                TypeDropdown dropdown = new TypeDropdown(types, MinDropdownWidth, MinDropdownLineCount, new AdvancedDropdownState());
                dropdown.OnItemSelected += handleItemSelected;
                dropdown.Show(position);
            }
        }

        private static void FilterTypes(SerializedProperty property, List<Type> types)
        {
            PropertyPathInfo scope = property.GetScope()!;
            TypeFilterAttribute typeFilterAttribute = scope.FieldInfo.GetCustomAttribute<TypeFilterAttribute>();

            if (typeFilterAttribute == null)
            {
                scope = scope.Scope;

                if (scope != null && scope.FieldInfo.FieldType!.GetGenericTypeDefinition() == typeof(Reference<>))
                {
                    typeFilterAttribute = scope.FieldInfo.GetCustomAttribute<TypeFilterAttribute>();
                }
            }

            if (typeFilterAttribute == null)
            {
                return;
            }

            MethodInfo methodInfo = scope.FieldInfo.DeclaringType!.FindMethodBySignature(typeFilterAttribute.MethodName, typeof(List<Type>));

            if (methodInfo == null)
            {
                return;
            }

            object[] parameters =
            {
                types,
            };

            if (scope.Scope == null)
            {
                foreach (Object o in property.serializedObject.targetObjects)
                {
                    methodInfo.Invoke(o, parameters);
                }
            }
            else
            {
                using (ListPool.Pop(out List<object> list))
                {
                    scope.Scope.GetValues(property.serializedObject.targetObjects, list);

                    foreach (object o in list)
                    {
                        methodInfo.Invoke(o, parameters);
                    }
                }
            }
        }

        private static Type GetType(string typeName)
        {
            int index = typeName.IndexOf(' ');
            Assembly assembly = Assembly.Load(typeName.Substring(0, index));

            return assembly.GetType(typeName.Substring(index + 1));
        }
    }
}
