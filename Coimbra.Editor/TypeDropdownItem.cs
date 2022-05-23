using System;
using UnityEditor.IMGUI.Controls;

namespace Coimbra.Editor
{
    internal sealed class TypeDropdownItem : AdvancedDropdownItem
    {
        internal readonly Type Type;

        public TypeDropdownItem(Type type, string name)
            : base(name)
        {
            Type = type;
        }
    }
}
