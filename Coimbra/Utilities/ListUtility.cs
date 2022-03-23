using System.Collections.Generic;

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
        public static void Clear<T>(this List<T> list, int minCapacity)
        {
            list.Clear();

            if (list.Capacity < minCapacity)
            {
                list.Capacity = minCapacity;
            }
        }
    }
}
