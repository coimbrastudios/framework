using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Listen to <see cref="LateUpdate"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Late Update Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.LateUpdate.html")]
    public sealed class LateUpdateListener : PlayerLoopListenerBase
    {
        /// <inheritdoc />
        public override event EventHandler OnTrigger
        {
            add
            {
                base.OnTrigger += value;
                enabled = HasListener;
            }
            remove
            {
                base.OnTrigger -= value;
                enabled = HasListener;
            }
        }

        private void Awake()
        {
            enabled = HasListener;
        }

        private void LateUpdate()
        {
            Trigger(Time.deltaTime);
        }
    }
}
