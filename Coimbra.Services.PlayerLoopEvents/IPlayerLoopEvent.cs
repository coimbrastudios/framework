using Coimbra.Services.Events;
using UnityEngine.Scripting;

namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Base event interface for any event that occurs per frame.
    /// </summary>
    [RequireImplementors]
    public interface IPlayerLoopEvent : IEvent
    {
        /// <summary>
        /// Cached value for <see cref="UnityEngine.Time.deltaTime"/>.
        /// </summary>
        float DeltaTime { get; }
    }
}
