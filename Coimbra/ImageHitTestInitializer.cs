using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace Coimbra
{
    /// <summary>
    /// Component to initialize an <see cref="Image"/>'s <see cref="Image.alphaHitTestMinimumThreshold"/> on <see cref="Awake"/>.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Image Hit Test Initialized")]
    [HelpURL("https://docs.unity3d.com/Packages/com.unity.ugui@1.0/api/UnityEngine.UI.Image.html#UnityEngine_UI_Image_alphaHitTestMinimumThreshold")]
    public sealed class ImageHitTestInitializer : MonoBehaviour
    {
        [SerializeField]
        [Range(0, 1)]
        [Tooltip("The alpha threshold specifies the minimum alpha a pixel must have for the event to considered a 'hit' on the Image.")]
        private float _minimumThreshold = 0.5f;

        /// <inheritdoc cref="Image.alphaHitTestMinimumThreshold"/>
        public float MinimumThreshold
        {
            [DebuggerStepThrough]
            get => _minimumThreshold;
            set => _minimumThreshold = Mathf.Clamp01(value);
        }

        private void Awake()
        {
            GetComponent<Image>().alphaHitTestMinimumThreshold = MinimumThreshold;
        }
    }
}
