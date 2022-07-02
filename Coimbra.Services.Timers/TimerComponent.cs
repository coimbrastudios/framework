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
        private static readonly ProfilerCounterValue<int> ActiveTimers = new(ProfilerCategory.Scripts, "Active Timers", ProfilerMarkerDataUnit.Count, ProfilerCounterOptions.FlushOnEndOfFrame);

        [field: SerializeField]
        [field: Disable]
        internal TimerHandle Handle { get; set; }

        internal float Delay { get; set; }

        internal float Rate { get; set; }

        internal int TargetLoops { get; set; }

        internal int CompletedLoops { get; set; }

        internal Action Callback { get; set; }

        internal void Run()
        {
            Callback.Invoke();

            CompletedLoops++;

            if (TargetLoops > 0 && CompletedLoops == TargetLoops)
            {
                Handle.Service.StopTimer(Handle);
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
