using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Listen to <see cref="Collider2D"/>'s <see cref="OnCollisionStay2D"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Collision Stay 2D Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnCollisionStay2D.html")]
    public sealed class CollisionStay2DListener : Collision2DListenerBase
    {
        private void OnCollisionStay2D(Collision2D collision)
        {
            Trigger(collision);
        }
    }
}
