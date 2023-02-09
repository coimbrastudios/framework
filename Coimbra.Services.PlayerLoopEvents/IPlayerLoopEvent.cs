using Coimbra.Services.Events;
using UnityEngine.Scripting;

namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Base event interface for any event that occurs per frame.
    /// </summary>
    /// <seealso cref="IPlayerLoopService"/>
    /// <seealso cref="PlayerLoopSystem"/>
    /// <seealso cref="PlayerLoopEventListener"/>
    /// <seealso cref="FixedUpdateEvent"/>
    /// <seealso cref="UpdateEvent"/>
    /// <seealso cref="LateUpdateEvent"/>
    /// <seealso cref="FirstInitializationEvent"/>
    /// <seealso cref="LastInitializationEvent"/>
    /// <seealso cref="FirstEarlyUpdateEvent"/>
    /// <seealso cref="LastEarlyUpdateEvent"/>
    /// <seealso cref="FirstFixedUpdateEvent"/>
    /// <seealso cref="LastFixedUpdateEvent"/>
    /// <seealso cref="FirstPreUpdateEvent"/>
    /// <seealso cref="LastPreUpdateEvent"/>
    /// <seealso cref="FirstUpdateEvent"/>
    /// <seealso cref="LastUpdateEvent"/>
    /// <seealso cref="PreLateUpdateEvent"/>
    /// <seealso cref="FirstPostLateUpdateEvent"/>
    /// <seealso cref="PostLateUpdateEvent"/>
    /// <seealso cref="LastPostLateUpdateEvent"/>
    /// <seealso cref="PreTimeUpdateEvent"/>
    /// <seealso cref="PostTimeUpdateEvent"/>
    [RequireImplementors]
    [AllowEventServiceUsageFor(typeof(IPlayerLoopService))]
    public interface IPlayerLoopEvent : IEvent
    {
        /// <summary>
        /// Gets cached value for <see cref="UnityEngine.Time.deltaTime"/>.
        /// </summary>
        float DeltaTime { get; }
    }
}
