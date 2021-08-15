using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Provides easy access to Unity's <see cref="MonoBehaviour.Invoke"/> and <see cref="MonoBehaviour.InvokeRepeating"/> system without requiring the object to be an <see cref="MonoBehaviour"/>.
    /// </summary>
    public interface ITimerService
    {
        /// <summary>
        /// Is the timer still valid and running?
        /// </summary>
        /// <param name="timerHandle">It will get reset if not valid anymore.</param>
        /// <returns>True if the timer is still valid and running.</returns>
        bool IsTimerActive(ref TimerHandle timerHandle);

        /// <summary>
        /// Starts a new timer.
        /// </summary>
        /// <param name="timerHandle">It will stop the previously timer if still valid then override it with the new timer handle.</param>
        /// <param name="callback">What should happen when the timer finishes.</param>
        /// <param name="duration">The timer duration.</param>
        void StartTimer(ref TimerHandle timerHandle, Action callback, float duration);

        /// <summary>
        /// Starts a new timer.
        /// </summary>
        /// <param name="timerHandle">It will stop the previously timer if still valid then override it with the new timer handle.</param>
        /// <param name="callback">What should happen each time the timer triggers.</param>
        /// <param name="delay">The delay to trigger the first time.</param>
        /// <param name="rate">The interval between each trigger.</param>
        /// <param name="loops">The amount of times it should trigger. Infinite if 0 or negative.</param>
        void StartTimer(ref TimerHandle timerHandle, Action callback, float delay, float rate, int loops = 0);

        /// <summary>
        /// Stops an existing timer if still valid.
        /// </summary>
        /// <param name="timerHandle">Ignored if not valid anymore.</param>
        void StopTimer(ref TimerHandle timerHandle);

        /// <summary>
        /// Stops all existing timers.
        /// </summary>
        void StopAllTimers();
    }

    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    internal sealed class TimerService : MonoBehaviour, ITimerService
    {
        private sealed class TimerContextPool : ManagedPoolBase<TimerInstance>
        {
            private readonly GameObject _gameObject;

            public TimerContextPool(GameObject gameObject)
            {
                _gameObject = gameObject;
            }

            protected override TimerInstance OnCreate()
            {
                return _gameObject.AddComponent<TimerInstance>();
            }

            protected override void OnDelete(TimerInstance item) { }

            protected override void OnGet(TimerInstance item)
            {
                item.enabled = true;
            }

            protected override void OnRelease(TimerInstance item)
            {
                item.enabled = false;
            }
        }

        private readonly Dictionary<TimerHandle, TimerInstance> _instances = new Dictionary<TimerHandle, TimerInstance>();

        private TimerContextPool Pool { get; set; }

        public bool IsTimerActive(ref TimerHandle timerHandle)
        {
            if (_instances.TryGetValue(timerHandle, out TimerInstance context))
            {
                return context.enabled;
            }

            timerHandle.Id = 0;

            return false;
        }

        public void StartTimer(ref TimerHandle timerHandle, Action callback, float duration)
        {
            if (_instances.TryGetValue(timerHandle, out TimerInstance context))
            {
                context.CancelInvoke();
            }
            else
            {
                context = Pool.Get();
            }

            context.Version++;
            context.CompletedLoops = 0;
            context.TargetLoops = 1;
            context.Callback = callback;
            context.Service = this;
            timerHandle.Id = context.GetInstanceID();
            timerHandle.Version = context.Version;
            _instances[timerHandle] = context;
            context.Invoke(nameof(TimerInstance.Run), duration);
        }

        public void StartTimer(ref TimerHandle timerHandle, Action callback, float delay, float rate, int loops = 0)
        {
            if (_instances.TryGetValue(timerHandle, out TimerInstance context))
            {
                context.CancelInvoke();
            }
            else
            {
                context = Pool.Get();
            }

            context.Version++;
            context.CompletedLoops = 0;
            context.TargetLoops = loops;
            context.Callback = callback;
            context.Service = this;
            timerHandle.Id = context.GetInstanceID();
            timerHandle.Version = context.Version;
            _instances[timerHandle] = context;
            context.InvokeRepeating(nameof(TimerInstance.Run), delay, rate);
        }

        public void StopTimer(ref TimerHandle timerHandle)
        {
            if (_instances.TryGetValue(timerHandle, out TimerInstance context))
            {
                Pool.Release(context);
                _instances.Remove(timerHandle);
            }

            timerHandle.Id = 0;
        }

        public void StopAllTimers()
        {
            foreach (KeyValuePair<TimerHandle, TimerInstance> pair in _instances)
            {
                Pool.Release(pair.Value);
            }

            _instances.Clear();
        }

        internal void StopTimer(TimerInstance instance)
        {
            Pool.Release(instance);
            _instances.Remove(new TimerHandle(instance.GetInstanceID(), instance.Version));
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            if (ServiceLocator.Global.IsCreated<ITimerService>() || ServiceLocator.Global.HasCreateCallback<ITimerService>())
            {
                return;
            }

            ServiceLocator.Global.SetCreateCallback(Create, true);
        }

        private static ITimerService Create()
        {
            GameObject gameObject = new GameObject(nameof(TimerService));
            TimerService service = gameObject.AddComponent<TimerService>();
            service.Pool = new TimerContextPool(gameObject);
            DontDestroyOnLoad(gameObject);

            return service;
        }
    }
}
