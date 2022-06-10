using System.Runtime.CompilerServices;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Listen to <see cref="Update"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Update Listener")]
    public sealed class UpdateListener : MonoBehaviour
    {
        public delegate void EventHandler(float deltaTime);

        /// <summary>
        /// Invoked inside <see cref="Update"/>.
        /// </summary>
        public event EventHandler OnInvoke
        {
            add
            {
                _callback += value;
                ValidateEnabledState();
            }
            remove
            {
                _callback -= value;
                ValidateEnabledState();
            }
        }

        private EventHandler _callback;

        private void OnValidate()
        {
            ValidateEnabledState();
        }

        private void Update()
        {
            _callback?.Invoke(Time.deltaTime);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ValidateEnabledState()
        {
            enabled = _callback != null;
        }
    }
}
