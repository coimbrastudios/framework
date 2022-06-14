using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Listen to <see cref="Collider"/>'s <see cref="OnCollisionExit"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Collision Exit Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnCollisionExit.html")]
    public sealed class CollisionExitListener : CollisionListenerBase
    {
        private void OnCollisionExit(Collision collision)
        {
            Trigger(collision);
        }
    }
}
