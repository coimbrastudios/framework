#nullable enable

using System.Collections;
using System.Collections.Generic;

namespace Coimbra
{
    /// <summary>
    /// Default comparer for any <see cref="ISortable"/>.
    /// </summary>
    public sealed class SortableComparer : IComparer<ISortable>, IComparer
    {
        /// <summary>
        /// Default instance that can be safely reused.
        /// </summary>
        public static readonly SortableComparer Default = new();

        /// <inheritdoc/>
        public int Compare(ISortable? x, ISortable? y)
        {
            if (ReferenceEquals(x, y))
            {
                return 0;
            }

            if (ReferenceEquals(null, y))
            {
                return 1;
            }

            if (ReferenceEquals(null, x))
            {
                return -1;
            }

            return x.Order.CompareTo(y.Order);
        }

        /// <inheritdoc/>
        public int Compare(object x, object y)
        {
            return Compare(x as ISortable, y as ISortable);
        }
    }
}
