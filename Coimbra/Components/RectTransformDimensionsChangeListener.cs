using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Listen to <see cref="RectTransform"/>'s <see cref="OnRectTransformDimensionsChange"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Rect Transform Dimensions Change Listener")]
    [HelpURL("https://docs.unity3d.com/Packages/com.unity.ugui@1.0/api/UnityEngine.EventSystems.UIBehaviour.html#UnityEngine_EventSystems_UIBehaviour_OnRectTransformDimensionsChange")]
    public sealed class RectTransformDimensionsChangeListener : MonoBehaviour
    {
        public delegate void EventHandler(RectTransformDimensionsChangeListener sender);

        /// <summary>
        /// Invoked inside <see cref="OnRectTransformDimensionsChange"/>.
        /// </summary>
        public event EventHandler OnTrigger;

        private RectTransform _rectTransform;

        /// <summary>
        /// The rect transform this component depends on.
        /// </summary>
        public RectTransform RectTransform => _rectTransform != null ? _rectTransform : _rectTransform = GetComponent<RectTransform>();

        private void OnRectTransformDimensionsChange()
        {
            OnTrigger?.Invoke(this);
        }
    }
}
