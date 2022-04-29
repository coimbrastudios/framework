#nullable enable

using System;

namespace Coimbra.Inspectors
{
    /// <summary>
    /// Shows a value in the inspector besides not being supported by Unity.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class ShowInInspectorAttribute : Attribute
    {
        public DecoratorConditions? Conditions { get; set; } = null;

        public string? Predicate { get; set; } = null;
    }
}
