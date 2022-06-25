using Coimbra.Editor;
using System.Diagnostics;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners.Editor
{
    /// <summary>
    /// Editor for <see cref="RenderObjectListener"/>.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(RenderObjectListener))]
    [MovedFrom(true, "Coimbra.Editor", "Coimbra.Editor")]
    public sealed class RenderObjectListenerEditor : UnityEditor.Editor
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
