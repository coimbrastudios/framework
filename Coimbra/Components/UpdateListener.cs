using System.Runtime.CompilerServices;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Listen to <see cref="Update"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Update Listener")]
    public sealed class UpdateListener : PlayerLoopListenerBase
    {
        private void Update()
        {
            Trigger();
        }
    }
}
