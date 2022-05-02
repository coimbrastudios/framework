#nullable enable

using System.Runtime.CompilerServices;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Utility methods for <see cref="Rect"/> type.
    /// </summary>
    public static class RectUtility
    {
        /// <summary>
        /// Creates a copy of the rect with the specified y.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect WithY(in this Rect rect, float y)
        {
            Rect copy = rect;
            copy.y = y;

            return copy;
        }

        /// <summary>
        /// Creates a copy of the rect with the specified height.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect WithHeight(in this Rect rect, float height)
        {
            Rect copy = rect;
            copy.height = height;

            return copy;
        }
    }
}
