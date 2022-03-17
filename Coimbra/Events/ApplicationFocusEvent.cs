using UnityEngine.Scripting;

namespace Coimbra
{
    [Preserve]
    public readonly struct ApplicationFocusEvent
    {
        public readonly bool IsFocused;

        public ApplicationFocusEvent(bool isFocused)
        {
            IsFocused = isFocused;
        }
    }
}
