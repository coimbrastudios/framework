using System.Runtime.CompilerServices;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Listen to <see cref="FixedUpdate"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Fixed Update Listener")]
    public sealed class FixedUpdateListener : PlayerLoopListenerBase
    {
        private void FixedUpdate()
        {
            Trigger();
        }
    }
}
