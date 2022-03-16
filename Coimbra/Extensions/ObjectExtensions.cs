using System.Runtime.CompilerServices;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Extension methods for both Unity <see cref="Object"/> and C# <see cref="object"/> types.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Get a valid object to be used with ?. and ?? operators.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetValid<T>(this T o)
            where T : class
        {
            if (o is Object obj)
            {
                return obj != null ? o : null;
            }

            return o;
        }

        /// <summary>
        /// Safe way to check if an object is valid even if the object is an Unity <see cref="Object"/> and got destroyed already.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(this object o)
        {
            if (o is Object obj)
            {
                return obj != null;
            }

            return o != null;
        }
    }
}
