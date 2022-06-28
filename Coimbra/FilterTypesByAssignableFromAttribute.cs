using System;
using Object = UnityEngine.Object;

namespace Coimbra
{
    /// <summary>
    /// Filter the types using <see cref="Type.IsAssignableFrom"/>.
    /// </summary>
    /// <seealso cref="FilterTypesAttributeBase"/>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FilterTypesByAssignableFromAttribute : FilterTypesAttributeBase
    {
        /// <summary>
        /// The type needs to implement all those interfaces.
        /// </summary>
        public readonly Type[] All;

        public FilterTypesByAssignableFromAttribute(params Type[] all)
        {
            All = all;
            Any = null;
            None = null;
        }

        /// <summary>
        /// Gets or sets the types that at least one should be extended.
        /// </summary>
        public Type[] Any { get; set; }

        /// <summary>
        /// Gets or sets the types that should not be extended.
        /// </summary>
        public Type[] None { get; set; }

        /// <inheritdoc/>
        public override bool Validate(PropertyPathInfo context, Object[] targets, Type type)
        {
            if (!ValidateTypesInAll(type))
            {
                return false;
            }

            if (!ValidateTypesInNone(type))
            {
                return false;
            }

            if (Any == null || Any.Length == 0)
            {
                return true;
            }

            foreach (Type t in Any)
            {
                if (t.IsAssignableFrom(type))
                {
                    return true;
                }
            }

            return false;
        }

        private bool ValidateTypesInAll(Type type)
        {
            if (All == null)
            {
                return true;
            }

            foreach (Type t in All)
            {
                if (!t.IsAssignableFrom(type))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ValidateTypesInNone(Type type)
        {
            if (None == null)
            {
                return true;
            }

            foreach (Type t in None)
            {
                if (t.IsAssignableFrom(type))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
