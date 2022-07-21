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
        FirstInitialization = 1 << 0,

        /// <inheritdoc cref="LastInitializationEvent"/>
        LastInitialization = 1 << 1,

        /// <inheritdoc cref="FirstEarlyUpdateEvent"/>
        FirstEarlyUpdate = 1 << 2,

        /// <inheritdoc cref="LastEarlyUpdateEvent"/>
        LastEarlyUpdate = 1 << 3,

        /// <inheritdoc cref="FirstFixedUpdateEvent"/>
        FirstFixedUpdate = 1 << 4,

        /// <inheritdoc cref="LastFixedUpdateEvent"/>
        LastFixedUpdate = 1 << 5,

        /// <inheritdoc cref="FirstPreUpdateEvent"/>
        FirstPreUpdate = 1 << 6,

        /// <inheritdoc cref="LastPreUpdateEvent"/>
        LastPreUpdate = 1 << 7,

        /// <inheritdoc cref="FirstUpdateEvent"/>
        FirstUpdate = 1 << 8,

        /// <inheritdoc cref="LastUpdateEvent"/>
        LastUpdate = 1 << 9,

        /// <inheritdoc cref="PreLateUpdateEvent"/>
        PreLateUpdate = 1 << 10,

        /// <inheritdoc cref="FirstPostLateUpdateEvent"/>
        FirstPostLateUpdate = 1 << 11,

        /// <inheritdoc cref="PostLateUpdateEvent"/>
        PostLateUpdate = 1 << 12,

        /// <inheritdoc cref="LastPostLateUpdateEvent"/>
        LastPostLateUpdate = 1 << 13,

        /// <inheritdoc cref="PreTimeUpdateEvent"/>
        PreTimeUpdate = 1 << 14,

        /// <inheritdoc cref="PostTimeUpdateEvent"/>
        PostTimeUpdate = 1 << 15,
    }
}
