#nullable enable

using System;

namespace Coimbra.Inspectors
{
    /// <summary>
    /// Conditionally disables a field on the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class DisableIfAttribute : InspectorDecoratorAttributeBase
    {
        public readonly DecoratorConditions? Conditions;

        public readonly string? Predicate;

        public DisableIfAttribute(DecoratorConditions conditions = DecoratorConditions.Default)
        {
            Predicate = null;
            Conditions = conditions;
        }

        public DisableIfAttribute(string predicate, DecoratorConditions conditions = DecoratorConditions.Default)
        {
            Predicate = predicate;
            Conditions = conditions;
        }
    }
}
