using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="Collider"/>'s <see cref="OnTriggerEnter"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraListenersUtility.PhysicsMenuPath + "Trigger Enter Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnTriggerEnter.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class TriggerEnterListener : TriggerListenerBase
    {
        private void OnTriggerEnter(Collider other)
        {
            Trigger(other);
        }
    }
}
