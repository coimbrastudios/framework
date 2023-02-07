using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Coimbra
{
    /// <summary>
    /// Apply this to a field with <see cref="SerializeReference"/> to expose a type dropdown for it.
    /// </summary>
    /// <remarks>
    /// For a type appear in the dropdown it needs to satisfy the following conditions:
    /// <list type="bullet">
    /// <item>Not be abstract</item>
    /// <item>Not be generic</item>
    /// <item>Not be assignable to <see cref="Object"/></item>
    /// <item>Have the <see cref="SerializableAttribute"/></item>
    /// <item>Be a value type or have a parameterless constructor</item>
    /// </list>
    /// It is compatible with <see cref="FilterTypesAttributeBase"/> attributes.
    /// </remarks>
    /// <seealso cref="FilterTypesAttributeBase"/>
    /// <seealso cref="FilterTypesByAccessibilityAttribute"/>
    /// <seealso cref="FilterTypesByMethodAttribute"/>
    /// <seealso cref="FilterTypesByAssignableFromAttribute"/>
    /// <seealso cref="FilterTypesBySpecificTypeAttribute"/>
    public sealed class TypeDropdownAttribute : ValidateAttribute { }
}
