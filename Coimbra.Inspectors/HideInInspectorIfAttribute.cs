#nullable enable

using System;

namespace Coimbra.Inspectors
{
    /// <summary>
    /// Conditionally hides a field on the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class HideInInspectorIfAttribute : Attribute
    {
        public readonly DecoratorConditions? Conditions;

        public readonly string? Predicate;

        public HideInInspectorIfAttribute(DecoratorConditions conditions)
        {
            Predicate = null;
            Conditions = conditions;
        }

        public HideInInspectorIfAttribute(string predicate, DecoratorConditions conditions = DecoratorConditions.Default)
        {
            Predicate = predicate;
            Conditions = conditions;
        }
    }
}
