namespace Coimbra
{
    /// <summary>
    /// Display a message in the inspector while in edit mode.
    /// </summary>
    /// <seealso cref="MessageBoxAttribute"/>
    /// <seealso cref="MessageBoxOnPlayModeAttribute"/>
    public sealed class MessageBoxOnEditModeAttribute : MessageBoxAttributeBase
    {
        public MessageBoxOnEditModeAttribute(string message, InspectorArea area)
            : base(message, area) { }

        public MessageBoxOnEditModeAttribute(string message, MessageBoxType type = MessageBoxType.None, InspectorArea area = InspectorArea.Fill)
            : base(message, type, area) { }

        /// <inheritdoc/>
        public override bool ShouldDisplayMessageBox()
        {
            return ApplicationUtility.IsEditMode;
        }
    }
}
