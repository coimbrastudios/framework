using System;
using Object = UnityEngine.Object;

namespace Coimbra
{
    /// <summary>
    /// Implement this to filter which types should be available to either a <see cref="ManagedField{T}"/>, or <see cref="Reference{T}"/>, or a field with <see cref="TypeDropdownAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public abstract class TypeFilterAttributeBase : Attribute
    {
        /// <summary>
        /// Return true if the <paramref name="type"/> should be allowed, false otherwise.
        /// </summary>
        public abstract bool Validate(PropertyPathInfo context, Object[] targets, Type type);
    }
}
