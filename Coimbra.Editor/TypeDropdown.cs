using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Coimbra.Editor
{
    internal sealed class TypeDropdown : AdvancedDropdown
    {
        internal event Action<TypeDropdownItem> OnItemSelected;

        private const int MinLineCount = 10;

        private const float MinWidth = 400;

        private static SerializedProperty _current;

        private readonly Type[] _types;

        public TypeDropdown(IEnumerable<Type> types, AdvancedDropdownState state)
            : base(state)
        {
            _types = types.ToArray();
            minimumSize = new Vector2(MinWidth, EditorGUIUtility.singleLineHeight * MinLineCount + EditorGUIUtility.singleLineHeight * 2);
        }

        internal static void DrawReferenceField(Rect position, Type type, SerializedProperty property, GUIContent label, string undoKey, Action<List<Type>> filterCallback)
        {
            if (!EditorGUI.DropdownButton(position, label, FocusType.Keyboard))
            {
                return;
            }

            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

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

                    if (!derivedType.IsValueType && derivedType.GetConstructor(bindingFlags, null, Type.EmptyTypes, null) == null)
                    {
                        continue;
                    }

                    types.Add(derivedType);
                }

                _current = property;

                void handleItemSelected(TypeDropdownItem item)
                {
                    Undo.RecordObjects(_current.serializedObject.targetObjects, undoKey);

                    if (item.Type == null)
                    {
                        _current.SetValues(null);
                    }
                    else
                    {
                        _current.SetValues(true, delegate
                        {
                            try
                            {
                                return Activator.CreateInstance(item.Type);
                            }
                            catch
                            {
                                return item.Type.GetConstructor(bindingFlags, null, Type.EmptyTypes, null)!.Invoke(null);
                            }
                        });
                    }

                    _current.isExpanded = item.Type != null;
                    _current.serializedObject.ApplyModifiedProperties();
                    _current.serializedObject.Update();
                }

                filterCallback(types);

                TypeDropdown dropdown = new TypeDropdown(types, new AdvancedDropdownState());
                dropdown.OnItemSelected += handleItemSelected;
                dropdown.Show(position);
            }
        }

        internal static void FilterTypes(Object[] targets, PropertyPathInfo propertyPathInfo, List<Type> types)
        {
            TypeFilterAttributeBase typeFilterAttribute = propertyPathInfo.FieldInfo.GetCustomAttribute<TypeFilterAttributeBase>();

            if (typeFilterAttribute == null)
            {
                propertyPathInfo = propertyPathInfo.Scope;

                if (propertyPathInfo == null)
                {
                    return;
                }

                Type fieldType = propertyPathInfo.FieldInfo.FieldType!;

                if (!fieldType.IsGenericType || fieldType.GetGenericTypeDefinition() != typeof(Reference<>))
                {
                    return;
                }

                typeFilterAttribute = propertyPathInfo.FieldInfo.GetCustomAttribute<TypeFilterAttributeBase>();

                if (typeFilterAttribute == null)
                {
                    return;
                }
            }

            for (int i = types.Count - 1; i >= 0; i--)
            {
                if (!typeFilterAttribute.Validate(propertyPathInfo, targets, types[i]))
                {
                    types.RemoveAt(i);
                }
            }
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            int itemCount = 0;
            AdvancedDropdownItem root = new AdvancedDropdownItem("Select Type");

            TypeDropdownItem defaultItem = new TypeDropdownItem(null, "<null>")
            {
                id = itemCount++
            };

            root.AddChild(defaultItem);

            foreach (Type type in _types)
            {
                TypeDropdownItem item = new TypeDropdownItem(type, TypeString.Get(type))
                {
                    id = itemCount++
                };

                root.AddChild(item);
            }

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            base.ItemSelected(item);

            if (item is TypeDropdownItem typePopupItem)
            {
                OnItemSelected?.Invoke(typePopupItem);
            }
        }
    }
}
