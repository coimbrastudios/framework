using System;

namespace Coimbra
{
    /// <summary>
    /// Filter the types using their accessibility.
    /// </summary>
    /// <remarks>
    /// By adding this attribute any internal type not present in <see cref="IncludedInternalTypes"/> will be hidden.
    /// </remarks>
    /// <seealso cref="FilterTypesAttributeBase"/>
    /// <seealso cref="FilterTypesByMethodAttribute"/>
    /// <seealso cref="FilterTypesByAssignableFromAttribute"/>
    /// <seealso cref="FilterTypesBySpecificTypeAttribute"/>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FilterTypesByAccessibilityAttribute : FilterTypesAttributeBase
    {
        /// <summary>
        /// The internal types to be included.
        /// </summary>
        public readonly Type[] IncludedInternalTypes;

        public FilterTypesByAccessibilityAttribute(params Type[] includedInternalTypes)
        {
            IncludedInternalTypes = includedInternalTypes;
        }

        /// <inheritdoc/>
        public override bool Validate(PropertyPathInfo context, UnityEngine.Object[] targets, Type type)
        {
            if (type.IsVisible)
            {
                return true;
            }

            if (IncludedInternalTypes == null)
            {
                return false;
            }

            foreach (Type t in IncludedInternalTypes)
            {
                if (t.IsAssignableFrom(type))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
