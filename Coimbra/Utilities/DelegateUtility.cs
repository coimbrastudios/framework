#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Utility methods for <see cref="Delegate"/>.
    /// </summary>
    public static class DelegateUtility
    {
        /// <summary>
        /// Gets the list of targets and theirs methods to be invoked.
        /// </summary>
        /// <param name="handler">The delegate instance.</param>
        /// <param name="invocations">The list to add the pairs of target and method name.</param>
        /// <returns>The amount of items added to <paramref name="invocations"/>.</returns>
        public static int GetInvocationList(this Delegate? handler, List<DelegateInfo> invocations)
        {
            if (handler == null)
            {
                return 0;
            }

            Delegate[] handlers = handler.GetInvocationList();

            foreach (Delegate item in handlers)
            {
                invocations.Add(new DelegateInfo(in item));
            }

            return handlers.Length;
        }
    }
}
