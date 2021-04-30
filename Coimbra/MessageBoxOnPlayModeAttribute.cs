namespace Coimbra
{
    /// <summary>
    ///     Display a message in the inspector while in play mode.
    /// </summary>
    public sealed class MessageBoxOnPlayModeAttribute : MessageBoxAttribute
    {
        public MessageBoxOnPlayModeAttribute(string message, MessageBoxType type)
            : base(message, type) { }

        public MessageBoxOnPlayModeAttribute(string message, bool fillLabelArea = true, MessageBoxType type = MessageBoxType.None)
            : base(message, fillLabelArea, type) { }
    }
}
