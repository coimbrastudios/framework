using UnityEngine.Scripting;

namespace Coimbra.Services
{
    /// <summary>
    /// Base event interface for any event that occurs per frame.
    /// </summary>
    [RequireImplementors]
    public interface IApplicationLoopEvent : IEvent
    {
        /// <summary>
        /// Cached value for <see cref="UnityEngine.Time.deltaTime"/>.
        /// </summary>
        float DeltaTime { get; }
    }
}
