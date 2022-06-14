using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Listen to <see cref="Collider2D"/>'s <see cref="OnTriggerExit2D"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Trigger Exit 2D Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnTriggerExit2D.html")]
    public sealed class TriggerExit2DListener : Trigger2DListenerBase
    {
        private void OnTriggerExit2D(Collider2D other)
        {
            Trigger(other);
        }
    }
}
