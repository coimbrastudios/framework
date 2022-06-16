using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Listen to <see cref="Collider"/>'s <see cref="OnTriggerStay"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Trigger Stay Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnTriggerStay.html")]
    public sealed class TriggerStayListener : TriggerListenerBase
    {
        private void OnTriggerStay(Collider other)
        {
            Trigger(other);
        }
    }
}
