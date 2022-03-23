using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Default implementation for <see cref="ITimerService"/>.
    /// </summary>
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    internal sealed class TimerSystem : MonoBehaviourServiceBase<ITimerService>, ITimerService
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

            /// <inheritdoc/>
            protected override TimerComponent OnCreate()
            {
                TimerComponent instance = _gameObject.AddComponent<TimerComponent>();
                instance.Service = _service;

                return instance;
            }

            /// <inheritdoc/>
            protected override void OnDelete(TimerComponent item)
            {
                if (item != null)
                {
                    Destroy(item);
                }
            }

            /// <inheritdoc/>
            protected override void OnGet(TimerComponent item)
            {
                item.enabled = true;
            }

            /// <inheritdoc/>
            protected override void OnRelease(TimerComponent item)
            {
                item.enabled = false;
            }
        }

        private readonly Dictionary<TimerHandle, TimerComponent> _instances = new Dictionary<TimerHandle, TimerComponent>();

        private TimerComponentPool Pool { get; set; }

        /// <inheritdoc/>
        public bool IsTimerActive(in TimerHandle timerHandle)
        {
            return _instances.TryGetValue(timerHandle, out TimerComponent context) && context.enabled;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void StopAllTimers()
        {
            foreach (KeyValuePair<TimerHandle, TimerComponent> pair in _instances)
            {
                Pool.Release(pair.Value);
            }

            _instances.Clear();
        }

        /// <inheritdoc/>
        public void StopTimer(in TimerHandle timerHandle)
        {
            if (!_instances.TryGetValue(timerHandle, out TimerComponent context))
            {
                return;
            }

            _instances.Remove(timerHandle);
            Pool.Release(context);
        }

        /// <inheritdoc/>
        protected override void OnDispose()
        {
            base.OnDispose();
            StopAllTimers();
            Pool.Reset();
        }

        internal static ITimerService Create()
        {
            GameObject gameObject = new GameObject(nameof(TimerSystem));
            TimerSystem system = gameObject.AddComponent<TimerSystem>();
            system.Pool = new TimerComponentPool(gameObject, system);
            DontDestroyOnLoad(gameObject);

            return system;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            ServiceLocator.SetDefaultCreateCallback(Create, false);
        }
    }
}
