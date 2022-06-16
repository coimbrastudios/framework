using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Listen to <see cref="Collider2D"/>'s <see cref="OnCollisionExit2D"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Collision Exit 2D Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnCollisionExit2D.html")]
    public sealed class CollisionExit2DListener : Collision2DListenerBase
    {
        private void OnCollisionExit2D(Collision2D collision)
        {
            Trigger(collision);
        }
    }
}
