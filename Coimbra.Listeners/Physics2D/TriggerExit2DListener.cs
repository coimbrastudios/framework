using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="Collider2D"/>'s <see cref="OnTriggerExit2D"/> callback.
    /// </summary>
    /// <seealso cref="Collider2DParticleCollisionListener"/>
    /// <seealso cref="ColliderOverlap2DListener"/>
    /// <seealso cref="CollisionEnter2DListener"/>
    /// <seealso cref="CollisionExit2DListener"/>
    /// <seealso cref="CollisionStay2DListener"/>
    /// <seealso cref="JointBreak2DListener"/>
    /// <seealso cref="RigidbodyOverlap2DListener"/>
    /// <seealso cref="TriggerEnter2DListener"/>
    /// <seealso cref="TriggerStay2DListener"/>
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
