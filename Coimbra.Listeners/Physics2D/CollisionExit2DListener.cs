using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="Collider2D"/>'s <see cref="OnCollisionExit2D"/> callback.
    /// </summary>
    /// <seealso cref="Collider2DParticleCollisionListener"/>
    /// <seealso cref="ColliderOverlap2DListener"/>
    /// <seealso cref="CollisionEnter2DListener"/>
    /// <seealso cref="CollisionStay2DListener"/>
    /// <seealso cref="JointBreak2DListener"/>
    /// <seealso cref="RigidbodyOverlap2DListener"/>
    /// <seealso cref="TriggerEnter2DListener"/>
    /// <seealso cref="TriggerExit2DListener"/>
    /// <seealso cref="TriggerStay2DListener"/>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraListenersUtility.Physics2DMenuPath + "Collision Exit 2D Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnCollisionExit2D.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class CollisionExit2DListener : Collision2DListenerBase
    {
        private void OnCollisionExit2D(Collision2D collision)
        {
            Trigger(collision);
        }
    }
}
