using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="Collider"/>'s <see cref="OnCollisionExit"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraListenersUtility.PhysicsMenuPath + "Collision Exit Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnCollisionExit.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class CollisionExitListener : CollisionListenerBase
    {
        private void OnCollisionExit(Collision collision)
        {
            Trigger(collision);
        }
    }
}
