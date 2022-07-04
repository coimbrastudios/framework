using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="Collider"/>'s <see cref="OnCollisionStay"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraListenersUtility.PhysicsMenuPath + "Collision Stay Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnCollisionStay.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class CollisionStayListener : CollisionListenerBase
    {
        private void OnCollisionStay(Collision collision)
        {
            Trigger(collision);
        }
    }
}
