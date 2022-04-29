#nullable enable

namespace Coimbra.Inspectors
{
    /// <summary>
    /// Display a message in the inspector.
    /// </summary>
    public sealed class MessageBoxAttribute : InspectorDecoratorAttributeBase
    {
        public readonly string Message;

        public readonly MessageBoxType Type;

        public MessageBoxAttribute(string message, MessageBoxType type = MessageBoxType.None)
        {
            Message = message;
            Type = type;
        }

        public InspectorArea Area { get; set; } = 0;

        public DecoratorPosition Position { get; set; } = 0;

        public DecoratorConditions? Conditions { get; set; } = null;

        public string? Predicate { get; set; } = null;
    }
}
