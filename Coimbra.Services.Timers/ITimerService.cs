#nullable enable

using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace Coimbra.Services.Timers
{
    /// <summary>
    /// Provides easy access to Unity's <see cref="MonoBehaviour.Invoke"/> and <see cref="MonoBehaviour.InvokeRepeating"/> system without requiring the object to be an <see cref="MonoBehaviour"/>.
    /// </summary>
    [RequiredService]
    [RequireImplementors]
    public interface ITimerService : IService
    {
        /// <summary>
        /// Get the timer data for the given timer handle.
        /// </summary>
        /// <param name="timerHandle">The timer to get the data.</param>
        /// <param name="callback">Gets the callback configured for the given timer.</param>
        /// <param name="delay">Gets the configured delay for the given timer.</param>
        /// <param name="rate">Gets the configured rate for the given timer. If no rate was configured, will return -1.</param>
        /// <param name="targetLoops">Gets the configured loops for the given timer. If no loops was configured, will return 1.</param>
        /// <param name="completedLoops">Gets the amount of completed loops for the given timer.</param>
        /// <returns>True if the timer is still valid and running.</returns>
        bool GetTimerData(in TimerHandle timerHandle, out Action? callback, out float delay, out float rate, out int targetLoops, out int completedLoops);

        /// <summary>
        /// Checks if the timer still valid and running.
        /// </summary>
        /// <param name="timerHandle">The timer to check.</param>
        /// <returns>True if the timer is still valid and running.</returns>
        bool IsTimerActive(in TimerHandle timerHandle);

        /// <summary>
        /// Starts a new timer that fires only once.
        /// </summary>
        /// <param name="callback">What should happen when the timer finishes.</param>
        /// <param name="delay">The timer duration. Is set to 0 if negative.</param>
        TimerHandle StartTimer(in Action callback, float delay);

        /// <summary>
        /// Starts a new timer that can fire multiple times.
        /// </summary>
        /// <param name="callback">What should happen each time the timer triggers.</param>
        /// <param name="delay">The delay to trigger the first time. Is set to 0 if negative.</param>
        /// <param name="rate">The interval between each trigger. Each frame if 0 or negative..</param>
        /// <param name="loops">The amount of times it should trigger. Infinite if 0 or negative.</param>
        TimerHandle StartTimer(in Action callback, float delay, float rate, int loops = 0);

        /// <summary>
        /// Stops all existing timers.
        /// </summary>
        void StopAllTimers();

        /// <summary>
        /// Stops an existing timer if still valid.
        /// </summary>
        /// <param name="timerHandle">Ignored if not valid anymore.</param>
        void StopTimer(in TimerHandle timerHandle);
    }
}
