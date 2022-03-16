using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Provides easy access to Unity's <see cref="MonoBehaviour.Invoke"/> and <see cref="MonoBehaviour.InvokeRepeating"/> system without requiring the object to be an <see cref="MonoBehaviour"/>.
    /// </summary>
    [RequireImplementors]
    public interface ITimerService : IService
    {
        /// <summary>
        /// Is the timer still valid and running?
        /// </summary>
        /// <param name="timerHandle">It will get reset if not valid anymore.</param>
        /// <returns>True if the timer is still valid and running.</returns>
        bool IsTimerActive(in TimerHandle timerHandle);

        /// <summary>
        /// Starts a new timer.
        /// </summary>
        /// <param name="callback">What should happen when the timer finishes.</param>
        /// <param name="duration">The timer duration.</param>
        TimerHandle StartTimer(Action callback, float duration);

        /// <summary>
        /// Starts a new timer.
        /// </summary>
        /// <param name="callback">What should happen each time the timer triggers.</param>
        /// <param name="delay">The delay to trigger the first time.</param>
        /// <param name="rate">The interval between each trigger.</param>
        /// <param name="loops">The amount of times it should trigger. Infinite if 0 or negative.</param>
        TimerHandle StartTimer(Action callback, float delay, float rate, int loops = 0);

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
