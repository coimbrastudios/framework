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
            private readonly TimerService _service;

            public TimerContextPool(GameObject gameObject, TimerService service)
            {
                _gameObject = gameObject;
                _service = service;
            }

            protected override TimerContext OnCreate()
            {
                TimerContext instance = _gameObject.AddComponent<TimerContext>();
                instance.Service = _service;

                return instance;
            }

            protected override void OnDelete(TimerContext item)
            {
                if (item != null)
                {
                    Destroy(item);
                }
            }

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
        public bool IsTimerActive(in TimerHandle timerHandle)
        {
            return _instances.TryGetValue(timerHandle, out TimerContext context) && context.enabled;
        }

        /// <inheritdoc cref="ITimerService.StartTimer(System.Action, float)"/>>
        public TimerHandle StartTimer(Action callback, float duration)
        {
            TimerContext context = Pool.Get();
            TimerHandle handle = new TimerHandle(Guid.NewGuid());
            context.CompletedLoops = 0;
            context.TargetLoops = 1;
            context.Callback = callback;
            context.Handle = handle;
            _instances[handle] = context;
            context.Invoke(nameof(TimerContext.Run), duration);

            return handle;
        }

        /// <inheritdoc cref="ITimerService.StartTimer(System.Action, float, float, int)"/>>
        public TimerHandle StartTimer(Action callback, float delay, float rate, int loops = 0)
        {
            TimerContext context = Pool.Get();
            TimerHandle handle = new TimerHandle(Guid.NewGuid());
            context.CompletedLoops = 0;
            context.TargetLoops = loops;
            context.Callback = callback;
            context.Handle = handle;
            _instances[handle] = context;
            context.InvokeRepeating(nameof(TimerContext.Run), delay, rate);

            return handle;
        }

        /// <inheritdoc cref="ITimerService.StopTimer"/>>
        public void StopTimer(in TimerHandle timerHandle)
        {
            if (!_instances.TryGetValue(timerHandle, out TimerContext context))
            {
                return;
            }

            _instances.Remove(timerHandle);
            Pool.Release(context);
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

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            ServiceLocator.SetDefaultCreateCallback(Create, false);
        }

        private static ITimerService Create()
        {
            GameObject gameObject = new GameObject(nameof(TimerService));
            TimerService service = gameObject.AddComponent<TimerService>();
            service.Pool = new TimerContextPool(gameObject, service);
            DontDestroyOnLoad(gameObject);

            return service;
        }
    }
}
