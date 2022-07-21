using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="Collider2D"/>'s <see cref="OnCollisionEnter2D"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraListenersUtility.Physics2DMenuPath + "Collision Enter 2D Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnCollisionEnter2D.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class CollisionEnter2DListener : Collision2DListenerBase
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            Trigger(collision);
        }
    }
}
