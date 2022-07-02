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

        private readonly string _defaultValue;

        private readonly Type[] _types;

        public TypeDropdown(IEnumerable<Type> types, AdvancedDropdownState state, string defaultValue = "<null>")
            : base(state)
        {
            _defaultValue = defaultValue;
            _types = types.ToArray();
            minimumSize = new Vector2(MinWidth, (EditorGUIUtility.singleLineHeight * MinLineCount) + (EditorGUIUtility.singleLineHeight * 2));
        }

        internal static void DrawReferenceField(Rect position, Type type, SerializedProperty property, GUIContent label, string undoKey, Action<List<Type>> filterCallback)
        {
            if (!EditorGUI.DropdownButton(position, label, FocusType.Keyboard))
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

                    if (derivedType.CanCreateInstance())
                    {
                        types.Add(derivedType);
                    }
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
                            return item.Type.CreateInstance();
                        });
                    }

                    _current.isExpanded = item.Type != null;
                    _current.serializedObject.ApplyModifiedProperties();
                    _current.serializedObject.Update();
                }

                filterCallback(types);

                TypeDropdown dropdown = new(types, new AdvancedDropdownState());
                dropdown.OnItemSelected += handleItemSelected;
                dropdown.Show(position);
            }
        }

        internal static void FilterTypes(Object[] targets, PropertyPathInfo propertyPathInfo, List<Type> types)
        {
            IEnumerable<FilterTypesAttributeBase> filterTypesAttributes;

            if (propertyPathInfo.Depth > 0 && propertyPathInfo.ScopeInfo!.PropertyType.IsGenericType && propertyPathInfo.ScopeInfo.PropertyType.GetGenericTypeDefinition() == typeof(Reference<>))
            {
                filterTypesAttributes = propertyPathInfo.ScopeInfo.FieldInfo.GetCustomAttributes<FilterTypesAttributeBase>();
            }
            else
            {
                filterTypesAttributes = propertyPathInfo.FieldInfo.GetCustomAttributes<FilterTypesAttributeBase>();
            }

            foreach (FilterTypesAttributeBase filterTypeAttribute in filterTypesAttributes)
            {
                for (int i = types.Count - 1; i >= 0; i--)
                {
                    if (!filterTypeAttribute.Validate(propertyPathInfo, targets, types[i]))
                    {
                        types.RemoveAt(i);
                    }
                }
            }
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            int itemCount = 0;
            AdvancedDropdownItem root = new("Select Type");

            TypeDropdownItem defaultItem = new(null, _defaultValue)
            {
                id = itemCount++,
            };

            root.AddChild(defaultItem);

            foreach (Type type in _types)
            {
                TypeDropdownItem item = new(type, TypeString.Get(type))
                {
                    id = itemCount++,
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
