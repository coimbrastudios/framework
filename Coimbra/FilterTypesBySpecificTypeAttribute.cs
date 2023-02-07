using System;
using Object = UnityEngine.Object;

namespace Coimbra
{
    /// <summary>
    /// Filter the types using specific types.
    /// </summary>
    /// <remarks>
    /// Use this attribute to filter out very specific types.
    /// </remarks>
    /// <seealso cref="FilterTypesAttributeBase"/>
    /// <seealso cref="FilterTypesByAccessibilityAttribute"/>
    /// <seealso cref="FilterTypesByAssignableFromAttribute"/>
    /// <seealso cref="FilterTypesByMethodAttribute"/>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FilterTypesBySpecificTypeAttribute : FilterTypesAttributeBase
    {
        /// <summary>
        /// The types to be excluded.
        /// </summary>
        public readonly Type[] ExcludedTypes;

        public FilterTypesBySpecificTypeAttribute(params Type[] excludedTypes)
        {
            ExcludedTypes = excludedTypes;
        }

        /// <inheritdoc/>
        public override bool Validate(PropertyPathInfo context, Object[] targets, Type type)
        {
            if (ExcludedTypes == null)
            {
                return true;
            }

            foreach (Type t in ExcludedTypes)
            {
                if (t == type)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
