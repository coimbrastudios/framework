using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Listen to <see cref="Collider"/>'s <see cref="OnTriggerExit"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Trigger Exit Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnTriggerExit.html")]
    public sealed class TriggerExitListener : TriggerListenerBase
    {
        private void OnTriggerExit(Collider other)
        {
            Trigger(other);
        }
    }
}
