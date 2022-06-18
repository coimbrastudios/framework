using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Coimbra
{
    /// <summary>
    /// Implement this to filter which types should be available. This attribute only has effect in the following fields:
    /// <list type="bullet">
    /// <item><see cref="ManagedField{T}"/></item>
    /// <item><see cref="SerializableType{T}"/></item>
    /// <item><see cref="Reference{T}"/> of <see cref="ManagedField{T}"/></item>
    /// <item><see cref="Reference{T}"/> of <see cref="SerializableType{T}"/></item>
    /// <item><see cref="SerializeReference"/> with <see cref="TypeDropdownAttribute"/></item>
    /// </list>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public abstract class FilterTypesAttributeBase : Attribute
    {
        /// <summary>
        /// Return true if the <paramref name="type"/> should be allowed, false otherwise.
        /// </summary>
        public abstract bool Validate(PropertyPathInfo context, Object[] targets, Type type);
    }
}
