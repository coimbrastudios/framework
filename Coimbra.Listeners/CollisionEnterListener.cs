using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="Collider"/>'s <see cref="OnCollisionEnter"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraListenersUtility.PhysicsMenuPath + "Collision Enter Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnCollisionEnter.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class CollisionEnterListener : CollisionListenerBase
    {
        private void OnCollisionEnter(Collision collision)
        {
            Trigger(collision);
        }
    }
}
