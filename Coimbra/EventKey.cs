using System;

namespace Coimbra
{
    /// <summary>
    /// The event key to be used in <see cref="IEventService"/> APIs.
    /// </summary>
    public sealed class EventKey
    {
        /// <summary>
        /// The restrictions of an <see cref="EventKey"/>.
        /// </summary>
        [Flags]
        public enum RestrictionOptions
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

        /// <summary>
        /// The restrictions being applied.
        /// </summary>
        public readonly RestrictionOptions Restrictions;

        public EventKey(RestrictionOptions restrictions = RestrictionOptions.All)
        {
            Restrictions = restrictions;
        }
    }
}
