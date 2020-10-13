namespace Coimbra
{
    /// <summary>
    ///     Display a message in the inspector while in play mode.
    /// </summary>
    public sealed class MessageBoxOnPlayModeAttribute : MessageBoxAttribute
    {
        public MessageBoxOnPlayModeAttribute(string message, MessageType type)
            : base(message, type) { }

        public MessageBoxOnPlayModeAttribute(string message, bool fillLabelArea = true, MessageType type = MessageType.None)
            : base(message, fillLabelArea, type) { }
    }
}
