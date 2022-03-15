using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace Coimbra.Services
{
    [AddComponentMenu("")]
    [Preserve]
    internal sealed class TimerContext : MonoBehaviour
    {
        internal int CompletedLoops;

        internal int TargetLoops;

        internal Action Callback;

        internal TimerHandle Handle;

        internal TimerService Service;

        internal void Run()
        {
            Callback?.Invoke();

            if (TargetLoops <= 0)
            {
                return;
            }

            CompletedLoops++;

            if (CompletedLoops < TargetLoops)
            {
                return;
            }

            Service.StopTimer(ref Handle);
        }

        private void OnDisable()
        {
            CancelInvoke();

            Callback = null;
        }
    }
}
