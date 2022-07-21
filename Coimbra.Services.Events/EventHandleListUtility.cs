using System.Collections.Generic;

namespace Coimbra.Services.Events
{
    /// <summary>
    /// Utility methods for <see cref="List{T}"/> of <see cref="EventHandle"/>.
    /// </summary>
    public static class EventHandleListUtility
    {
        /// <summary>
        /// Calls <see cref="IEventService.RemoveListener"/> for each element in the <paramref name="list"/> and then clears it.
        /// </summary>
        /// <returns>True if removed any valid listener.</returns>
        public static bool RemoveListenersAndClear(this IList<EventHandle> list)
        {
            IEventService eventService = ServiceLocator.GetChecked<IEventService>();
            bool hasRemovedAny = false;
            int count = list.Count;

            for (int i = 0; i < count; i++)
            {
                hasRemovedAny |= eventService.RemoveListener(list[i]);
            }

            list.Clear();

            return hasRemovedAny;
        }
    }
}
