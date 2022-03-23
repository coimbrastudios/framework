﻿namespace Coimbra
{
    /// <summary>
    /// Invoked during <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.FixedUpdate.html">FixedUpdate</a>.
    /// </summary>
    public readonly struct FixedUpdateEvent : IPlayerLoopEvent
    {
        /// <inheritdoc/>
        public float DeltaTime { get; }

        public FixedUpdateEvent(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
