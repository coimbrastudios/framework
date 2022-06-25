using Coimbra.Editor;
using System.Diagnostics;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners.Editor
{
    /// <summary>
    /// Editor for <see cref="RenderImageListener"/>.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(RenderImageListener))]
    [MovedFrom(true, "Coimbra.Editor", "Coimbra.Editor")]
    public sealed class RenderImageListenerEditor : UnityEditor.Editor
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
        [Conditional("WITH_URP")]
        private void DisplayRenderPipelineWarning()
        {
            CoimbraGUIUtility.DrawComponentWarningForRenderPipeline(target.GetType());
        }
    }
}
