using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Listen to <see cref="FixedUpdate"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Fixed Update Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.FixedUpdate.html")]
    public sealed class FixedUpdateListener : PlayerLoopListenerBase
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

        private void FixedUpdate()
        {
            Trigger(Time.deltaTime);
        }
    }
}
