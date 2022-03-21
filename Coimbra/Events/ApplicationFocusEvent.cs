using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Invoked during <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnApplicationFocus.html">OnApplicationFocus</a>.
    /// </summary>
    [Preserve]
    public readonly struct ApplicationFocusEvent
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
