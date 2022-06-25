using System.Diagnostics;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;

namespace Coimbra.UI
{
    /// <summary>
    /// Component to initialize an <see cref="Image"/>'s <see cref="UnityEngine.UI.Image.alphaHitTestMinimumThreshold"/> on <see cref="Awake"/>.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Image Hit Test Initialized")]
    [HelpURL("https://docs.unity3d.com/Packages/com.unity.ugui@1.0/api/UnityEngine.UI.Image.html#UnityEngine_UI_Image_alphaHitTestMinimumThreshold")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class ImageHitTestInitializer : MonoBehaviour
    {
        [SerializeField]
        [Range(0, 1)]
        [Tooltip("The alpha threshold specifies the minimum alpha a pixel must have for the event to considered a 'hit' on the Image.")]
        private float _minimumThreshold = 0.5f;

        private Image _image;

        /// <summary>
        /// The image this component depends on.
        /// </summary>
        public Image Image => _image != null ? _image : _image = GetComponent<Image>();

        /// <inheritdoc cref="UnityEngine.UI.Image.alphaHitTestMinimumThreshold"/>
        public float MinimumThreshold
        {
            [DebuggerStepThrough]
            get => _minimumThreshold;
            set
            {
                _minimumThreshold = Mathf.Clamp01(value);
                Image.alphaHitTestMinimumThreshold = _minimumThreshold;
            }
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                Image.alphaHitTestMinimumThreshold = MinimumThreshold;
            }
        }

        private void Awake()
        {
            Image.alphaHitTestMinimumThreshold = MinimumThreshold;
        }
    }
}
