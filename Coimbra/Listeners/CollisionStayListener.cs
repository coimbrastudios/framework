using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Listen to <see cref="Collider"/>'s <see cref="OnCollisionStay"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Collision Stay Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnCollisionStay.html")]
    public sealed class CollisionStayListener : CollisionListenerBase
    {
        private void OnCollisionStay(Collision collision)
        {
            Trigger(collision);
        }
    }
}
