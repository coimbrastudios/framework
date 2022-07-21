using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="Collider"/>'s <see cref="OnTriggerExit"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraListenersUtility.PhysicsMenuPath + "Trigger Exit Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnTriggerExit.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class TriggerExitListener : TriggerListenerBase
    {
        private void OnTriggerExit(Collider other)
        {
            Trigger(other);
        }
    }
}
