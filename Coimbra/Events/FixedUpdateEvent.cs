using UnityEngine.Scripting;

namespace Coimbra
{
    [Preserve]
    public readonly struct FixedUpdateEvent
    {
        public readonly float DeltaTime;

        public FixedUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
