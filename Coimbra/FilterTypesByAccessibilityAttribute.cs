using System;

namespace Coimbra
{
    /// <summary>
    /// Filter the types using their accessibility.
    /// </summary>
    /// <seealso cref="FilterTypesAttributeBase"/>
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
