using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="Collider2D"/>'s <see cref="OnTriggerStay2D"/> callback.
    /// </summary>
    /// <seealso cref="Collider2DParticleCollisionListener"/>
    /// <seealso cref="ColliderOverlap2DListener"/>
    /// <seealso cref="CollisionEnter2DListener"/>
    /// <seealso cref="CollisionExit2DListener"/>
    /// <seealso cref="CollisionStay2DListener"/>
    /// <seealso cref="JointBreak2DListener"/>
    /// <seealso cref="RigidbodyOverlap2DListener"/>
    /// <seealso cref="TriggerEnter2DListener"/>
    /// <seealso cref="TriggerExit2DListener"/>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraListenersUtility.Physics2DMenuPath + "Trigger Stay 2D Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnTriggerStay2D.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class TriggerStay2DListener : Trigger2DListenerBase
    {
        private void OnTriggerStay2D(Collider2D other)
        {
            Trigger(other);
        }
    }
}
