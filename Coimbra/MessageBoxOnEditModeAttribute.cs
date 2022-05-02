namespace Coimbra
{
    /// <summary>
    /// Display a message in the inspector while in edit mode.
    /// </summary>
    public sealed class MessageBoxOnEditModeAttribute : MessageBoxAttribute
    {
        public MessageBoxOnEditModeAttribute(string message, InspectorArea area)
            : base(message, area) { }

        public MessageBoxOnEditModeAttribute(string message, MessageBoxType type = MessageBoxType.None, InspectorArea area = InspectorArea.Fill)
            : base(message, type, area) { }
    }
}
