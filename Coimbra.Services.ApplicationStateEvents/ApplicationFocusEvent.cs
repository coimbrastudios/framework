namespace Coimbra.Services.ApplicationStateEvents
{
    /// <summary>
    /// Invoked during <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnApplicationFocus.html">OnApplicationFocus</a>.
    /// </summary>
    public readonly partial struct ApplicationFocusEvent : IApplicationStateEvent
    {
        /// <summary>
        /// True if currently focused.
        /// </summary>
        public readonly bool IsFocused;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationFocusEvent"/> struct.
        /// </summary>
        /// <param name="isFocused">True if currently focused.</param>
        public ApplicationFocusEvent(bool isFocused)
        {
            IsFocused = isFocused;
        }
    }
}
