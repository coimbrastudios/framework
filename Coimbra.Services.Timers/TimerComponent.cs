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
        internal TimerHandle Handle;

        internal float Delay;

        internal float Rate;

        internal int TargetLoops;

        internal int CompletedLoops;

        internal Action Callback;

        internal void Run()
        {
            Callback.Invoke();

            CompletedLoops++;

            if (TargetLoops > 0 && CompletedLoops == TargetLoops)
            {
                Handle.Service.StopTimer(in Handle);
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
