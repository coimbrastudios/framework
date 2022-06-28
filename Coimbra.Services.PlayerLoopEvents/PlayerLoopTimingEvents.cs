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

        /// <inheritdoc cref="FirstInitializationEvent"/>
        FirstInitialization = 1,

        /// <inheritdoc cref="LastInitializationEvent"/>
        LastInitialization = 2,

        /// <inheritdoc cref="FirstEarlyUpdateEvent"/>
        FirstEarlyUpdate = 4,

        /// <inheritdoc cref="LastEarlyUpdateEvent"/>
        LastEarlyUpdate = 8,

        /// <inheritdoc cref="FirstFixedUpdateEvent"/>
        FirstFixedUpdate = 16,

        /// <inheritdoc cref="LastFixedUpdateEvent"/>
        LastFixedUpdate = 32,

        /// <inheritdoc cref="FirstPreUpdateEvent"/>
        FirstPreUpdate = 64,

        /// <inheritdoc cref="LastPreUpdateEvent"/>
        LastPreUpdate = 128,

        /// <inheritdoc cref="FirstUpdateEvent"/>
        FirstUpdate = 256,

        /// <inheritdoc cref="LastUpdateEvent"/>
        LastUpdate = 512,

        /// <inheritdoc cref="PreLateUpdateEvent"/>
        PreLateUpdate = 1024,

        /// <inheritdoc cref="FirstPostLateUpdateEvent"/>
        FirstPostLateUpdate = 2048,

        /// <inheritdoc cref="PostLateUpdateEvent"/>
        PostLateUpdate = 4096,

        /// <inheritdoc cref="LastPostLateUpdateEvent"/>
        LastPostLateUpdate = 8192,

        /// <inheritdoc cref="PreTimeUpdateEvent"/>
        PreTimeUpdate = 16384,

        /// <inheritdoc cref="PostTimeUpdateEvent"/>
        PostTimeUpdate = 32768,
    }
}
