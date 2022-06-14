using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Listen to <see cref="Collider"/>'s <see cref="OnTriggerEnter"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Trigger Enter Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnTriggerEnter.html")]
    public sealed class TriggerEnterListener : TriggerListenerBase
    {
        private void OnTriggerEnter(Collider other)
        {
            Trigger(other);
        }
    }
}
