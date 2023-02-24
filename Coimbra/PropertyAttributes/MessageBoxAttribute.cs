namespace Coimbra
{
    /// <summary>
    /// Always display a message in the inspector.
    /// </summary>
    /// <seealso cref="MessageBoxOnEditModeAttribute"/>
    /// <seealso cref="MessageBoxOnPlayModeAttribute"/>
    public sealed class MessageBoxAttribute : MessageBoxAttributeBase
    {
        public MessageBoxAttribute(string message, InspectorArea area)
            : base(message, area) { }

        public MessageBoxAttribute(string message, MessageBoxType type = MessageBoxType.None, InspectorArea area = InspectorArea.Fill)
            : base(message, type, area) { }

        /// <inheritdoc/>
        public override bool ShouldDisplayMessageBox()
        {
            return true;
        }
    }
}
