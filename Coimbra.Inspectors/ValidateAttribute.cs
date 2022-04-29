#nullable enable

using System;

namespace Coimbra.Inspectors
{
    /// <summary>
    /// Invokes an action expression each time the field changes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class ValidateAttribute : InspectorDecoratorAttributeBase
    {
        public readonly string Action;

        public ValidateAttribute(string action)
        {
            Action = action;
        }
    }
}
