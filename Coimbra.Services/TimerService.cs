using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coimbra.Services
{
    /// <summary>
    /// Default implementation for <see cref="ITimerService"/>.
    /// </summary>
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public class TimerService : MonoBehaviour, ITimerService
    {
        private sealed class TimerContextPool : ManagedPoolBase<TimerContext>
        {
            private readonly GameObject _gameObject;

            public TimerContextPool(GameObject gameObject)
            {
                _gameObject = gameObject;
            }

            protected override TimerContext OnCreate()
            {
                return _gameObject.AddComponent<TimerContext>();
            }

            protected override void OnDelete(TimerContext item) { }

            protected override void OnGet(TimerContext item)
            {
                item.enabled = true;
            }

            protected override void OnRelease(TimerContext item)
            {
                item.enabled = false;
            }
        }

        private readonly Dictionary<TimerHandle, TimerContext> _instances = new Dictionary<TimerHandle, TimerContext>();

        private TimerContextPool Pool { get; set; }

        /// <inheritdoc cref="ITimerService.IsTimerActive"/>>
        public bool IsTimerActive(ref TimerHandle timerHandle)
        {
            if (_instances.TryGetValue(timerHandle, out TimerContext context))
            {
                return context.enabled;
            }

            timerHandle.Invalidate();

            return false;
        }

        /// <inheritdoc cref="ITimerService.StartTimer(ref Coimbra.TimerHandle, System.Action, float)"/>>
        public void StartTimer(ref TimerHandle timerHandle, Action callback, float duration)
        {
            if (_instances.TryGetValue(timerHandle, out TimerContext context))
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
            timerHandle.Initialize(context.GetInstanceID(), context.Version);

            _instances[timerHandle] = context;
            context.Invoke(nameof(TimerContext.Run), duration);
        }

        /// <inheritdoc cref="ITimerService.StartTimer(ref Coimbra.TimerHandle, System.Action, float, float, int)"/>>
        public void StartTimer(ref TimerHandle timerHandle, Action callback, float delay, float rate, int loops = 0)
        {
            if (_instances.TryGetValue(timerHandle, out TimerContext context))
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
            timerHandle.Initialize(context.GetInstanceID(), context.Version);

            _instances[timerHandle] = context;
            context.InvokeRepeating(nameof(TimerContext.Run), delay, rate);
        }

        /// <inheritdoc cref="ITimerService.StopTimer"/>>
        public void StopTimer(ref TimerHandle timerHandle)
        {
            if (_instances.TryGetValue(timerHandle, out TimerContext context))
            {
                Pool.Release(context);
                _instances.Remove(timerHandle);
            }

            timerHandle.Invalidate();
        }

        /// <inheritdoc cref="ITimerService.StopAllTimers"/>>
        public void StopAllTimers()
        {
            foreach (KeyValuePair<TimerHandle, TimerContext> pair in _instances)
            {
                Pool.Release(pair.Value);
            }

            _instances.Clear();
        }

        internal void StopTimer(TimerContext context)
        {
            Pool.Release(context);
            _instances.Remove(new TimerHandle(context.GetInstanceID(), context.Version));
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            ServiceLocator.SetDefaultCreateCallback(Create, false);
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
