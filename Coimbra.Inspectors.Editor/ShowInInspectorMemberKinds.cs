#nullable enable

using System;

namespace Coimbra.Inspectors.Editor
{
    /// <summary>
    /// The kinds of member with <see cref="ShowInInspectorAttribute"/>.
    /// </summary>
    [Flags]
    public enum ShowInInspectorMemberKinds
    {
        /// <summary>
        /// Static member.
        /// </summary>
        Static = 1 << 10,

        /// <summary>
        /// Instance member (non-static).
        /// </summary>
        Instance = 1 << 1,
    }
}
