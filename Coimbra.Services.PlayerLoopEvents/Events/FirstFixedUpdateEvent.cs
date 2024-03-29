﻿namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Invoked during <see cref="UnityEngine.PlayerLoop.FixedUpdate"/>.
    /// </summary>
    /// <seealso cref="IPlayerLoopEvent"/>
    public readonly partial struct FirstFixedUpdateEvent : IPlayerLoopEvent
    {
        public FirstFixedUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }

        /// <inheritdoc/>
        public float DeltaTime { get; }
    }
}
