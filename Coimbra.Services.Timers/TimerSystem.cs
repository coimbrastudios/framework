#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coimbra.Services.Timers
{
    /// <summary>
    /// Default implementation for <see cref="ITimerService"/>.
    /// </summary>
    [AddComponentMenu("")]
    public sealed class TimerSystem : Actor, ITimerService
    {
        private readonly Dictionary<TimerHandle, TimerComponent> _instances = new();

        [SerializeField]
        [Disable]
        private ManagedPool<TimerComponent> _timerComponentPool = null!;

        private TimerSystem() { }

        /// <inheritdoc/>
        public bool GetTimerData(in TimerHandle timerHandle, out Action? callback, out float delay, out float rate, out int targetLoops, out int completedLoops)
        {
            if (!_instances.TryGetValue(timerHandle, out TimerComponent context) || !context.enabled)
            {
                callback = null;
                delay = 0;
                rate = 0;
                targetLoops = 0;
                completedLoops = 0;

                return false;
            }

            callback = context.Callback;
            delay = context.Delay;
            rate = context.Rate;
            targetLoops = context.TargetLoops;
            completedLoops = context.CompletedLoops;

            return true;
        }

        /// <inheritdoc/>
        public bool IsTimerActive(in TimerHandle timerHandle)
        {
            return _instances.TryGetValue(timerHandle, out TimerComponent context) && context.enabled;
        }

        /// <inheritdoc/>
        public TimerHandle StartTimer(in Action callback, float delay)
        {
            TimerComponent component = _timerComponentPool.Pop();
            TimerHandle handle = TimerHandle.Create(this);
            component.Delay = Mathf.Max(delay, 0);
            component.Rate = -1;
            component.TargetLoops = 1;
            component.CompletedLoops = 0;
            component.Callback = callback;
            component.Handle = handle;
            _instances[handle] = component;
            component.Invoke(nameof(TimerComponent.Run), component.Delay);

            return handle;
        }

        /// <inheritdoc/>
        public TimerHandle StartTimer(in Action callback, float delay, float rate, int loops = 0)
        {
            TimerComponent component = _timerComponentPool.Pop();
            TimerHandle handle = TimerHandle.Create(this);
            component.Delay = Mathf.Max(delay, 0);
            component.Rate = Mathf.Max(rate, 0);
            component.TargetLoops = Mathf.Max(loops, 0);
            component.CompletedLoops = 0;
            component.Callback = callback;
            component.Handle = handle;
            _instances[handle] = component;
            component.InvokeRepeating(nameof(TimerComponent.Run), component.Delay, component.Rate);

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

        /// <inheritdoc/>
        protected override void OnDestroyed()
        {
            base.OnDestroyed();
            StopAllTimers();
            _timerComponentPool.Initialize(0, 0);
        }

        /// <inheritdoc/>
        protected override void OnInitialize()
        {
            base.OnInitialize();
            DontDestroyOnLoad(GameObject);

            static void disposeCallback(TimerComponent component)
            {
                if (component != null)
                {
                    Destroy(component);
                }
            }

            TimerComponent createCallback()
            {
                return GameObject.AddComponent<TimerComponent>();
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
    }
}
