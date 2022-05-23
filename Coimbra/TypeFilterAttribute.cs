using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Apply this to either a <see cref="ManagedField{T}"/>, or <see cref="Reference{T}"/>, or a field with <see cref="TypeDropdownAttribute"/> to filter which types should be available.
    /// </summary>
    public class TypeFilterAttribute : PropertyAttribute
    {
        /// <summary>
        /// Additional filter to be applied. It is required to be void and receive a <see cref="List{T}"/> of <see cref="Type"/> as the only parameter.
        /// </summary>
        public readonly string MethodName;

        /// <param name="methodName">Additional filter to be applied. It is required to be void and receive a List of Type as the only parameter.</param>
        public TypeFilterAttribute(string methodName = null)
        {
            MethodName = methodName;
        }
    }
}
