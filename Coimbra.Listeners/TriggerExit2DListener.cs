using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="Collider2D"/>'s <see cref="OnTriggerExit2D"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraListenersUtility.Physics2DMenuPath + "Trigger Exit 2D Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnTriggerExit2D.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class TriggerExit2DListener : Trigger2DListenerBase
    {
        private void OnTriggerExit2D(Collider2D other)
        {
            Trigger(other);
        }
    }
}
