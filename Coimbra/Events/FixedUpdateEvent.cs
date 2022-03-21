using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Invoked during <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.FixedUpdate.html">FixedUpdate</a>.
    /// </summary>
    [Preserve]
    public readonly struct FixedUpdateEvent
    {
        /// <summary>
        /// Cached value for <see cref="UnityEngine.Time.deltaTime"/>.
        /// </summary>
        public readonly float DeltaTime;

        public FixedUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
