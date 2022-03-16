using UnityEngine.Scripting;

namespace Coimbra
{
    [Preserve]
    public struct ApplicationFocusEvent
    {
        public readonly bool IsFocused;

        public ApplicationFocusEvent(bool isFocused)
        {
            IsFocused = isFocused;
        }
    }
}
