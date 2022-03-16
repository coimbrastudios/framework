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
    public class TimerSystem : MonoBehaviour, ITimerService
    {
        private sealed class TimerComponentPool : ManagedPoolBase<TimerComponent>
        {
            private readonly GameObject _gameObject;
            private readonly ITimerService _service;

            public TimerComponentPool(GameObject gameObject, ITimerService service)
            {
                _gameObject = gameObject;
                _service = service;
            }

            protected override TimerComponent OnCreate()
            {
                TimerComponent instance = _gameObject.AddComponent<TimerComponent>();
                instance.Service = _service;

                return instance;
            }

            protected override void OnDelete(TimerComponent item)
            {
                if (item != null)
                {
                    Destroy(item);
                }
            }

            protected override void OnGet(TimerComponent item)
            {
                item.enabled = true;
            }

            protected override void OnRelease(TimerComponent item)
            {
                item.enabled = false;
            }
        }

        private readonly Dictionary<TimerHandle, TimerComponent> _instances = new Dictionary<TimerHandle, TimerComponent>();

        private TimerComponentPool Pool { get; set; }

        /// <inheritdoc cref="ITimerService.IsTimerActive"/>>
        public bool IsTimerActive(in TimerHandle timerHandle)
        {
            return _instances.TryGetValue(timerHandle, out TimerComponent context) && context.enabled;
        }

        /// <inheritdoc cref="ITimerService.StartTimer(System.Action, float)"/>>
        public TimerHandle StartTimer(Action callback, float duration)
        {
            if (callback == null)
            {
                return new TimerHandle();
            }

            TimerComponent component = Pool.Get();
            TimerHandle handle = TimerHandle.Create();
            component.CompletedLoops = 0;
            component.TargetLoops = 1;
            component.Callback = callback;
            component.Handle = handle;
            _instances[handle] = component;
            component.Invoke(nameof(TimerComponent.Run), duration);

            return handle;
        }

        /// <inheritdoc cref="ITimerService.StartTimer(System.Action, float, float, int)"/>>
        public TimerHandle StartTimer(Action callback, float delay, float rate, int loops = 0)
        {
            if (callback == null)
            {
                return new TimerHandle();
            }

            TimerComponent component = Pool.Get();
            TimerHandle handle = TimerHandle.Create();
            component.CompletedLoops = 0;
            component.TargetLoops = loops;
            component.Callback = callback;
            component.Handle = handle;
            _instances[handle] = component;
            component.InvokeRepeating(nameof(TimerComponent.Run), delay, rate);

            return handle;
        }

        /// <inheritdoc cref="ITimerService.StopAllTimers"/>>
        public void StopAllTimers()
        {
            foreach (KeyValuePair<TimerHandle, TimerComponent> pair in _instances)
            {
                Pool.Release(pair.Value);
            }

            _instances.Clear();
        }

        /// <inheritdoc cref="ITimerService.StopTimer"/>>
        public void StopTimer(in TimerHandle timerHandle)
        {
            if (!_instances.TryGetValue(timerHandle, out TimerComponent context))
            {
                return;
            }

            _instances.Remove(timerHandle);
            Pool.Release(context);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            ServiceLocator.SetDefaultCreateCallback(Create, false);
        }

        private static ITimerService Create()
        {
            GameObject gameObject = new GameObject(nameof(TimerSystem))
            {
                hideFlags = HideFlags.NotEditable | HideFlags.DontSave,
            };

            TimerSystem system = gameObject.AddComponent<TimerSystem>();
            system.Pool = new TimerComponentPool(gameObject, system);
            DontDestroyOnLoad(gameObject);

            return system;
        }
    }
}
