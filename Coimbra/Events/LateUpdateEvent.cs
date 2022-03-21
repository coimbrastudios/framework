using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Invoked during <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.LateUpdate.html">LateUpdate</a>.
    /// </summary>
    [Preserve]
    public readonly struct LateUpdateEvent
    {
        /// <summary>
        /// Cached value for <see cref="UnityEngine.Time.deltaTime"/>.
        /// </summary>
        public readonly float DeltaTime;

        public LateUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
