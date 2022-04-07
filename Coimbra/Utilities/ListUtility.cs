using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Coimbra
{
    /// <summary>
    /// Utility methods for <see cref="List{T}"/> type.
    /// </summary>
    public static class ListUtility
    {
        /// <summary>
        /// Shorthand for clearing a list and ensuring a min capacity on it.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear<T>(this List<T> list, int minCapacity)
        {
            list.Clear();
            EnsureCapacity(list, minCapacity);
        }

        /// <summary>
        /// Shorthand for ensuring at least the specified capacity.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnsureCapacity<T>(this List<T> list, int minCapacity)
        {
            if (list.Capacity < minCapacity)
            {
                list.Capacity = minCapacity;
            }
        }

        /// <summary>
        /// Shorthand for removing the last element and returning it without checking if the operation is valid.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T PopUnsafe<T>(this List<T> list)
        {
            int index = list.Count - 1;
            T item = list[index];
            list.RemoveAt(index);

            return item;
        }
    }
}
