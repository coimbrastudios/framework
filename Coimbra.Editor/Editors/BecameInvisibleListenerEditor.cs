using System.Diagnostics;
using UnityEditor;
using UnityEngine.Rendering;

namespace Coimbra.Editor
{
    /// <summary>
    /// Editor for <see cref="BecameInvisibleListener"/>.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BecameInvisibleListener))]
    public sealed class BecameInvisibleListenerEditor : UnityEditor.Editor
    {
        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GraphicsSettings.currentRenderPipeline.IsValid())
            {
                DisplayRenderPipelineWarning();
            }
        }

        [Conditional("WITH_HDRP")]
        private void DisplayRenderPipelineWarning()
        {
            CoimbraGUIUtility.DrawComponentWarningForRenderPipeline(target.GetType());
        }
    }
}
