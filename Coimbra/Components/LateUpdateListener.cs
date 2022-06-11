using System.Runtime.CompilerServices;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Listen to <see cref="LateUpdate"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Late Update Listener")]
    public sealed class LateUpdateListener : PlayerLoopListenerBase
    {
        private void LateUpdate()
        {
            Trigger();
        }
    }
}
