using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Listen to <see cref="Update"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Update Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html")]
    public sealed class UpdateListener : PlayerLoopListenerBase
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

        private void Update()
        {
            Trigger(Time.deltaTime);
        }
    }
}
