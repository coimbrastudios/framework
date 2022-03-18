using UnityEngine.Scripting;

namespace Coimbra
{
    [Preserve]
    public readonly struct ApplicationPauseEvent
    {
        public readonly bool IsPaused;

        public ApplicationPauseEvent(bool isPaused)
        {
            IsPaused = isPaused;
        }
    }
}
