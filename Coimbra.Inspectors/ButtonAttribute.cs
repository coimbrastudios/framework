#nullable enable

namespace Coimbra.Inspectors
{
    /// <summary>
    /// Draws a button sequence alongside a field.
    /// </summary>
    public sealed class ButtonAttribute : InspectorDecoratorAttributeBase
    {
        public readonly string[] Methods;

        public ButtonAttribute(params string[] methods)
        {
            Methods = methods;
        }

        public InspectorArea Area { get; set; } = 0;

        public DecoratorPosition Position { get; set; } = 0;

        public DecoratorConditions? Conditions { get; set; } = null;

        public string? Predicate { get; set; } = null;
    }
}
