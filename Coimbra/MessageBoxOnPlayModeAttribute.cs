namespace Coimbra
{
    /// <summary>
    /// Display a message in the inspector while in play mode.
    /// </summary>
    public sealed class MessageBoxOnPlayModeAttribute : MessageBoxAttribute
    {
        public MessageBoxOnPlayModeAttribute(string message, InspectorArea area)
            : base(message, area) { }

        public MessageBoxOnPlayModeAttribute(string message, MessageBoxType type = MessageBoxType.None, InspectorArea area = InspectorArea.Fill)
            : base(message, type, area) { }
    }
}
