using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Invoked during <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html">Update</a>.
    /// </summary>
    [Preserve]
    public readonly struct UpdateEvent
    {
        /// <summary>
        /// Cached value for <see cref="UnityEngine.Time.deltaTime"/>.
        /// </summary>
        public readonly float DeltaTime;

        public UpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
