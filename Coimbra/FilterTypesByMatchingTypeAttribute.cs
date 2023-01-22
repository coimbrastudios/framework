using System;
using Object = UnityEngine.Object;

namespace Coimbra
{
    /// <summary>
    /// Filter out the types that matches the given types.
    /// </summary>
    /// <seealso cref="FilterTypesAttributeBase"/>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FilterTypesByMatchingTypeAttribute : FilterTypesAttributeBase
    {
        /// <summary>
        /// The types to be excluded.
        /// </summary>
        public readonly Type[] ExcludedTypes;

        public FilterTypesByMatchingTypeAttribute(params Type[] excludedTypes)
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
