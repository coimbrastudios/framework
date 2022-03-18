using UnityEngine.Scripting;

namespace Coimbra
{
    [Preserve]
    public readonly struct LateUpdateEvent
    {
        public readonly float DeltaTime;

        public LateUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
