namespace Coimbra
{
    /// <summary>
    /// Display a message in the inspector while in play mode.
    /// </summary>
    /// <seealso cref="MessageBoxAttribute"/>
    /// <seealso cref="MessageBoxOnEditModeAttribute"/>
    public sealed class MessageBoxOnPlayModeAttribute : MessageBoxAttributeBase
    {
        public MessageBoxOnPlayModeAttribute(string message, InspectorArea area)
            : base(message, area) { }

        public MessageBoxOnPlayModeAttribute(string message, MessageBoxType type = MessageBoxType.None, InspectorArea area = InspectorArea.Fill)
            : base(message, type, area) { }

        /// <inheritdoc/>
        public override bool ShouldDisplayMessageBox()
        {
            return ApplicationUtility.IsPlayMode;
        }
    }
}
