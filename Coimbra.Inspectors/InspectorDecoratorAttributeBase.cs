#nullable enable

using System;

namespace Coimbra.Inspectors
{
    /// <summary>
    /// Implement this interface on attributes meant to be used as decorators.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
    public abstract class InspectorDecoratorAttributeBase : Attribute, ISortable
    {
        /// Optional field to specify the order in which the decorators should be processed.
        public int Order { get; set; } = 0;
    }
}
