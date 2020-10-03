using JetBrains.Annotations;
using UnityEngine;

namespace Coimbra.Attributes
{
    /// <summary>
    ///     Similar to <a href="https://docs.unity3d.com/ScriptReference/TooltipAttribute.html">TooltipAttribute</a> but uses another member summary.
    /// </summary>
    public sealed class SummaryAttribute : PropertyAttribute
    {
        [CanBeNull]
        public readonly string Member;

        /// <param name="member">The name of the member with the summary to be inherited from.</param>
        public SummaryAttribute([CanBeNull] string member)
        {
            Member = member;
        }
    }
}
