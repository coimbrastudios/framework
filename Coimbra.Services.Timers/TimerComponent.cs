using System;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Scripting;

namespace Coimbra.Services.Timers
{
    [Preserve]
    [AddComponentMenu("")]
    internal sealed class TimerComponent : MonoBehaviour
    {
        private static readonly ProfilerCounterValue<int> ActiveTimers = new ProfilerCounterValue<int>(ProfilerCategory.Scripts, "Active Timers", ProfilerMarkerDataUnit.Count, ProfilerCounterOptions.FlushOnEndOfFrame);

        [SerializeField]
        [Disable]
        internal float Delay;

        [SerializeField]
        [Disable]
        internal float Rate;

        [SerializeField]
        [Disable]
        internal int TargetLoops;

        [SerializeField]
        [Disable]
        internal int CompletedLoops;

        internal ITimerService Service;

        internal TimerHandle Handle;

        internal Action Callback;

        internal void Run()
        {
            Callback.Invoke();

            CompletedLoops++;

            if (TargetLoops > 0 && CompletedLoops == TargetLoops)
            {
                Service.StopTimer(in Handle);
            }
        }

        private void OnEnable()
        {
            ActiveTimers.Value++;
        }

        private void OnDisable()
        {
            Callback = null;
            ActiveTimers.Value--;
            CancelInvoke();
        }
    }
}
