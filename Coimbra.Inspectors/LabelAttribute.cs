#nullable enable

using System;

namespace Coimbra.Inspectors
{
    /// <summary>
    /// Changes the label displayed in the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class LabelAttribute : Attribute
    {
        public readonly string? Label;

        public LabelAttribute(string? label)
        {
            Label = label;
        }
    }
}
