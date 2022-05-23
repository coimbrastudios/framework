using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Coimbra.Editor
{
    internal sealed class TypeDropdown : AdvancedDropdown
    {
        internal event Action<TypeDropdownItem> OnItemSelected;

        private readonly Type[] _types;

        public TypeDropdown(IEnumerable<Type> types, float minWidth, int minLineCount, AdvancedDropdownState state)
            : base(state)
        {
            _types = types.ToArray();
            minimumSize = new Vector2(minWidth, EditorGUIUtility.singleLineHeight * minLineCount + EditorGUIUtility.singleLineHeight * 2);
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
