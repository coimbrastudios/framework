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
        /// The type needs to extend at least one of those types.
        /// </summary>
        public Type[] Any { get; set; }

        /// <summary>
        /// The type should not extend any of those types.
        /// </summary>
        public Type[] None { get; set; }

        /// <inheritdoc/>
        public override bool Validate(PropertyPathInfo context, Object[] targets, Type type)
        {
            if (All != null)
            {
                foreach (Type t in All)
                {
                    if (!t.IsAssignableFrom(type))
                    {
                        return false;
                    }
                }
            }

            if (None != null)
            {
                foreach (Type t in None)
                {
                    if (t.IsAssignableFrom(type))
                    {
                        return false;
                    }
                }
            }

            if (Any != null && Any.Length > 0)
            {
                foreach (Type t in Any)
                {
                    if (t.IsAssignableFrom(type))
                    {
                        return true;
                    }
                }

                return false;
            }

            return true;
        }
    }
}
