using Cysharp.Threading.Tasks;
using System;

namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Equivalent to <see cref="InjectPlayerLoopTimings"/>, but with the actual <see cref="IPlayerLoopEvent"/> names.
    /// </summary>
    [Flags]
    public enum PlayerLoopTimingEvents
    {
        /// <summary>
        /// No events.
        /// </summary>
        None = 0,

        /// <summary>
        /// All events.
        /// </summary>
        All = ~0,

        /// <summary>
        /// Common events for complex systems.
        /// </summary>
        Standard = FirstInitialization
                 | FirstEarlyUpdate
                 | FirstFixedUpdate
                 | FirstPreUpdate
                 | FirstUpdate
                 | PreLateUpdate
                 | PostLateUpdate
                 | PreTimeUpdate,

        /// <summary>
        /// Common events for simple systems.
        /// </summary>
        Minimum = FirstFixedUpdate
                | FirstUpdate
                | LastPostLateUpdate,

        /// <summary>
        /// <see cref="FirstInitializationEvent"/>
        /// </summary>
        FirstInitialization = 1,

        /// <summary>
        /// <see cref="LastInitializationEvent"/>
        /// </summary>
        LastInitialization = 2,

        /// <summary>
        /// <see cref="FirstEarlyUpdateEvent"/>
        /// </summary>
        FirstEarlyUpdate = 4,

        /// <summary>
        /// <see cref="LastEarlyUpdateEvent"/>
        /// </summary>
        LastEarlyUpdate = 8,

        /// <summary>
        /// <see cref="FirstFixedUpdateEvent"/>
        /// </summary>
        FirstFixedUpdate = 16,

        /// <summary>
        /// <see cref="LastFixedUpdateEvent"/>
        /// </summary>
        LastFixedUpdate = 32,

        /// <summary>
        /// <see cref="FirstPreUpdateEvent"/>
        /// </summary>
        FirstPreUpdate = 64,

        /// <summary>
        /// <see cref="LastPreUpdateEvent"/>
        /// </summary>
        LastPreUpdate = 128,

        /// <summary>
        /// <see cref="FirstUpdateEvent"/>
        /// </summary>
        FirstUpdate = 256,

        /// <summary>
        /// <see cref="LastUpdateEvent"/>
        /// </summary>
        LastUpdate = 512,

        /// <summary>
        /// <see cref="PreLateUpdateEvent"/>
        /// </summary>
        PreLateUpdate = 1024,

        /// <summary>
        /// <see cref="FirstPostLateUpdateEvent"/>
        /// </summary>
        FirstPostLateUpdate = 2048,

        /// <summary>
        /// <see cref="PostLateUpdateEvent"/>
        /// </summary>
        PostLateUpdate = 4096,

        /// <summary>
        /// <see cref="LastPostLateUpdateEvent"/>
        /// </summary>
        LastPostLateUpdate = 8192,

        /// <summary>
        /// <see cref="PreTimeUpdateEvent"/>
        /// </summary>
        PreTimeUpdate = 16384,

        /// <summary>
        /// <see cref="PostTimeUpdateEvent"/>
        /// </summary>
        PostTimeUpdate = 32768
    }
}
