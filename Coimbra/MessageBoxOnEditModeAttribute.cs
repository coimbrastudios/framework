namespace Coimbra
{
    /// <summary>
    /// Display a message in the inspector while in edit mode.
    /// </summary>
    public sealed class MessageBoxOnEditModeAttribute : MessageBoxAttribute
    {
        public MessageBoxOnEditModeAttribute(string message, MessageBoxType type)
            : base(message, type) { }

        public MessageBoxOnEditModeAttribute(string message, bool fillLabelArea = true, MessageBoxType type = MessageBoxType.None)
            : base(message, fillLabelArea, type) { }
    }
}
