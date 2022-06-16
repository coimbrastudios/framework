using UnityEngine;

namespace Coimbra
{
    [DisallowMultipleComponent]
    [AddComponentMenu("")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.FixedUpdate.html")]
    internal sealed class FixedUpdateListener : PlayerLoopListenerBase
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
