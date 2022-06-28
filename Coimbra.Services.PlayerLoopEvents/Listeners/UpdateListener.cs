using Coimbra.Listeners;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Services.PlayerLoopEvents
{
    [DisallowMultipleComponent]
    [AddComponentMenu("")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html")]
    [MovedFrom(true, "Coimbra")]
    internal sealed class UpdateListener : PlayerLoopListenerBase
    {
        /// <inheritdoc />
        public override event EventHandler OnTrigger
        {
            add
            {
                base.OnTrigger += value;

                if (this.IsValid())
                {
                    enabled = HasListener;
                }
            }

            remove
            {
                base.OnTrigger -= value;

                if (this.IsValid())
                {
                    enabled = HasListener;
                }
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
