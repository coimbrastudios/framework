#nullable enable

using System;
using System.Collections.Generic;

namespace Coimbra
{
    /// <summary>
    /// Utility methods for <see cref="Delegate"/>.
    /// </summary>
    public static class DelegateUtility
    {
        /// <summary>
        /// Gets the list of <see cref="DelegateListener"/> for the given <paramref name="handler"/>.
        /// </summary>
        /// <param name="handler">The delegate instance.</param>
        /// <param name="listeners">The list of listeners.</param>
        /// <returns>The amount of items added to <paramref name="listeners"/>.</returns>
        public static int GetListeners(this Delegate? handler, List<DelegateListener> listeners)
        {
            if (handler == null)
            {
                return 0;
            }

            Delegate[] handlers = handler.GetInvocationList();

            foreach (Delegate item in handlers)
            {
                listeners.Add(new DelegateListener(in item));
            }

            return handlers.Length;
        }
    }
}
