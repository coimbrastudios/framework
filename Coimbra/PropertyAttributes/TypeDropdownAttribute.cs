using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Coimbra
{
    /// <summary>
    /// Apply this to a field with <see cref="SerializeReference"/> to expose a type dropdown for it. For a type appear in the dropdown it needs to satisfy the following conditions:
    /// <list type="bullet">
    /// <item>Not be abstract</item>
    /// <item>Not be generic</item>
    /// <item>Not be assignable to <see cref="Object"/></item>
    /// <item>Have the <see cref="SerializableAttribute"/></item>
    /// <item>Be a value type or have a parameterless constructor</item>
    /// </list>
    /// </summary>
    public sealed class TypeDropdownAttribute : ValidateAttribute { }
}
