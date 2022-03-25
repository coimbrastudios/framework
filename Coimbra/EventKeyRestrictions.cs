using System;

namespace Coimbra
{
    /// <summary>
    /// The restrictions of an <see cref="EventKey"/>.
    /// </summary>
    [Flags]
    public enum EventKeyRestrictions
    {
        /// <summary>
        /// No restrictions.
        /// </summary>
        None = 0,
        /// <summary>
        /// All restrictions.
        /// </summary>
        All = ~0,
        /// <summary>
        /// Disallow the Invoke APIs to be called.
        /// </summary>
        DisallowInvoke = 1 << 0,
        /// <summary>
        /// Disallow the RemoveAll APIs to be called.
        /// </summary>
        DisallowRemoveAll = 1 << 1,
    }
}
