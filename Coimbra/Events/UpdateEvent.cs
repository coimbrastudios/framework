using UnityEngine.Scripting;

namespace Coimbra
{
    [Preserve]
    public readonly struct UpdateEvent
    {
        public readonly float DeltaTime;

        public UpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
