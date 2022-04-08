using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Default implementation for <see cref="ITimerService"/>.
    /// </summary>
    [AddComponentMenu("")]
    public sealed class TimerSystem : ServiceActorBase<ITimerService>, ITimerService
    {
        private readonly Dictionary<TimerHandle, TimerComponent> _instances = new Dictionary<TimerHandle, TimerComponent>();

        private ManagedPool<TimerComponent> _timerComponentPool;

        private TimerSystem() { }

        /// <summary>
        /// Create a new <see cref="ITimerService"/>.
        /// </summary>
        public static ITimerService Create()
        {
            return new GameObject(nameof(TimerSystem)).AddComponent<TimerSystem>();
        }

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

            TimerComponent component = _timerComponentPool.Pop();
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

            TimerComponent component = _timerComponentPool.Pop();
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
                _timerComponentPool.Push(pair.Value);
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
            _timerComponentPool.Push(context);
        }

        protected override void OnDestroying()
        {
            base.OnDestroying();
            StopAllTimers();
            _timerComponentPool.Initialize(0, 0);
            _timerComponentPool = null;
        }

        /// <inheritdoc/>
        protected override void OnInitialize()
        {
            base.OnInitialize();
            DontDestroyOnLoad(CachedGameObject);

            static void disposeCallback(TimerComponent component)
            {
                if (component != null)
                {
                    Destroy(component);
                }
            }

            TimerComponent createCallback()
            {
                TimerComponent instance = CachedGameObject.AddComponent<TimerComponent>();
                instance.Service = this;

                return instance;
            }

            static void onPop(TimerComponent component)
            {
                component.enabled = true;
            }

            static void onPush(TimerComponent component)
            {
                component.enabled = false;
            }

            _timerComponentPool = new ManagedPool<TimerComponent>(createCallback, disposeCallback);
            _timerComponentPool.OnPop += onPop;
            _timerComponentPool.OnPush += onPush;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void HandleSubsystemRegistration()
        {
            ServiceLocator.Shared.SetCreateCallback(Create, false);
        }
    }
}
