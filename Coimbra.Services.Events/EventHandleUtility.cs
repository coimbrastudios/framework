#nullable enable

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Coimbra.Services.Events
{
    /// <summary>
    /// Utility methods for <see cref="EventHandle"/>.
    /// </summary>
    public static class EventHandleUtility
    {
        /// <summary>
        /// Adds an <see cref="EventHandle"/> to a <see cref="List{T}"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddTo(this EventHandle eventHandle, List<EventHandle> list)
        {
            list.Add(eventHandle);
        }
    }
}
