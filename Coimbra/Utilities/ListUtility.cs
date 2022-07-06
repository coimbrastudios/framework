using JetBrains.Annotations;
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
        /// Add the same value multiple times to the list.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddRange<T>(this List<T> list, T value, int count)
        {
            EnsureCapacity(list, list.Count + count);

            for (int i = 0; i < count; i++)
            {
                list.Add(value);
            }
        }

        /// <summary>
        /// Add the same value multiple times to the list.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddRange<T>(this List<T> list, int count, [NotNull] Func<int, T> valueGetter)
        {
            EnsureCapacity(list, list.Count + count);

            for (int i = 0; i < count; i++)
            {
                list.Add(valueGetter(i));
            }
        }

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
        /// Shorthand for getting the last item of the list.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Last<T>(this List<T> list)
        {
            return list[list.Count - 1];
        }

        /// <summary>
        /// Shorthand for removing the last element and returning it without checking if the operation is valid.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Pop<T>(this List<T> list)
        {
            T item = Last(list);
            RemoveLast(list);

            return item;
        }

        /// <summary>
        /// Shorthand for removing the last element.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveLast<T>(this List<T> list)
        {
            list.RemoveAt(list.Count - 1);
        }

        /// <summary>
        /// More efficient version of <see cref="List{T}.Remove"/> that doesn't care about preserving the order.
        /// </summary>
        public static int RemoveSwapBack<T>(this List<T> list, T item)
        {
            int index = list.IndexOf(item);

            if (index < 0)
            {
                return index;
            }

            RemoveAtSwapBack(list, index);

            return index;
        }

        /// <summary>
        /// More efficient version of <see cref="List{T}.RemoveAt"/> that doesn't care about preserving the order.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveAtSwapBack<T>(this List<T> list, int index)
        {
            list[index] = Last(list);
            RemoveLast(list);
        }

        /// <summary>
        /// More efficient version of <see cref="List{T}.RemoveAll"/> that doesn't care about preserving the order.
        /// </summary>
        public static int RemoveAllSwapBack<T>(this List<T> list, Predicate<T> match)
        {
            if (match == null)
            {
                return 0;
            }

            int count = 0;

            for (int i = 0; i < list.Count; i++)
            {
                if (!match.Invoke(list[i]))
                {
                    continue;
                }

                RemoveAtSwapBack(list, i);
                count++;
                --i;
            }

            return count;
        }

        /// <summary>
        /// More efficient version of <see cref="List{T}.RemoveRange"/> that doesn't care about preserving the order.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveRangeSwapBack<T>(this List<T> list, int index, int count)
        {
            for (int i = count - 1; i >= 0; i--)
            {
                RemoveAtSwapBack(list, index + count);
            }
        }
    }
}
