using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Listen to <see cref="Collider2D"/>'s <see cref="OnCollisionEnter2D"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Collision Enter 2D Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnCollisionEnter2D.html")]
    public sealed class CollisionEnter2DListener : Collision2DListenerBase
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            Trigger(collision);
        }
    }
}
