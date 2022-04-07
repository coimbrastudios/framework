using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace Coimbra
{
    [Preserve]
    [AddComponentMenu("")]
    internal sealed class TimerComponent : MonoBehaviour
    {
        internal int CompletedLoops;

        internal int TargetLoops;

        internal ITimerService Service;

        internal TimerHandle Handle;

        internal Action Callback;

        internal void Run()
        {
            Callback.Invoke();

            if (TargetLoops <= 0)
            {
                return;
            }

            CompletedLoops++;

            if (CompletedLoops < TargetLoops)
            {
                return;
            }

            Service.StopTimer(in Handle);
        }

        private void OnDisable()
        {
            CancelInvoke();

            Callback = null;
        }
    }
}
