namespace Coimbra.Services
{
    /// <summary>
    /// Invoked during <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnApplicationFocus.html">OnApplicationFocus</a>.
    /// </summary>
    public readonly struct ApplicationFocusEvent : IEvent
    {
        /// <summary>
        /// True if currently focused.
        /// </summary>
        public readonly bool IsFocused;

        /// <param name="isFocused">True if currently focused.</param>
        public ApplicationFocusEvent(bool isFocused)
        {
            IsFocused = isFocused;
        }
    }
}
