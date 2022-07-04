using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="Collider2D"/>'s <see cref="OnCollisionStay2D"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraListenersUtility.Physics2DMenuPath + "Collision Stay 2D Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnCollisionStay2D.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class CollisionStay2DListener : Collision2DListenerBase
    {
        private void OnCollisionStay2D(Collision2D collision)
        {
            Trigger(collision);
        }
    }
}
