using System.Runtime.CompilerServices;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Listen to <see cref="FixedUpdate"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "FixedUpdate Listener")]
    public sealed class FixedUpdateListener : MonoBehaviour
    {
        public delegate void EventHandler(float deltaTime);

        /// <summary>
        /// Invoked inside <see cref="FixedUpdate"/>.
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

        private void FixedUpdate()
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
