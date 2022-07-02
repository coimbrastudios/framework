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
    /// Drawer for <see cref="SerializableType{T}"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(SerializableType<>))]
    public class SerializableTypeDrawer : PropertyDrawer
    {
        private const string ChangeUndoKey = "Change Type Value";

        private static SerializedProperty _current;

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;

            PropertyPathInfo context = property.GetPropertyPathInfo();
            Object[] targets = property.serializedObject.targetObjects;

            using (new ShowMixedValueScope(context.HasMultipleDifferentValues(targets)))
            {
                using EditorGUI.PropertyScope propertyScope = new(position, label, property);
                position = EditorGUI.PrefixLabel(position, propertyScope.content);

                using (GUIContentPool.Pop(out GUIContent dropdownLabel))
                {
                    if (EditorGUI.showMixedValue)
                    {
                        dropdownLabel.text = "-";
                        dropdownLabel.tooltip = "Editing multiple different values.";
                    }
                    else
                    {
                        dropdownLabel.text = context.GetValue(targets[0])!.ToString();
                        dropdownLabel.tooltip = dropdownLabel.text;
                    }

                    DrawDropdown(position, property, dropdownLabel, context, targets);
                }
            }
        }

        private static void DrawDropdown(Rect position, SerializedProperty property, GUIContent label, PropertyPathInfo context, Object[] targets)
        {
            if (!EditorGUI.DropdownButton(position, label, FocusType.Keyboard))
            {
                return;
            }

            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            using (ListPool.Pop(out List<Type> types))
            {
                Type baseType = context.PropertyType.GenericTypeArguments[0];

                foreach (Type derivedType in TypeCache.GetTypesDerivedFrom(baseType))
                {
                    if (!derivedType.IsDefined(typeof(CompilerGeneratedAttribute)))
                    {
                        types.Add(derivedType);
                    }
                }

                TypeDropdown.FilterTypes(targets, context, types);

                _current = property;

                void handleItemSelected(TypeDropdownItem item)
                {
                    Undo.RecordObjects(_current.serializedObject.targetObjects, ChangeUndoKey);

                    Type[] constructorTypes =
                    {
                        typeof(Type),
                    };

                    object[] constructorParameters =
                    {
                        item.Type,
                    };

                    object value = context.PropertyType.GetConstructor(bindingFlags, null, constructorTypes, null)!.Invoke(constructorParameters);

                    _current.SetValues(value);
                    _current.serializedObject.ApplyModifiedProperties();
                    _current.serializedObject.Update();
                }

                TypeDropdown dropdown = new(types, new AdvancedDropdownState(), TypeString.Get(baseType));
                dropdown.OnItemSelected += handleItemSelected;
                dropdown.Show(position);
            }
        }
    }
}
