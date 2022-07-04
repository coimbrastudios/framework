using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="Collider"/>'s <see cref="OnTriggerStay"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraListenersUtility.PhysicsMenuPath + "Trigger Stay Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnTriggerStay.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class TriggerStayListener : TriggerListenerBase
    {
        private void OnTriggerStay(Collider other)
        {
            Trigger(other);
        }
    }
}
