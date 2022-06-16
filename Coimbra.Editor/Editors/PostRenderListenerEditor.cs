using System.Diagnostics;
using UnityEditor;
using UnityEngine.Rendering;

namespace Coimbra.Editor
{
    /// <summary>
    /// Editor for <see cref="PostRenderListener"/>.
    /// </summary>
    [CustomEditor(typeof(PostRenderListener))]
    public sealed class PostRenderListenerEditor : UnityEditor.Editor
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
            CoimbraEditorGUIUtility.DrawComponentWarningForRenderPipeline(target.GetType());
        }
    }
}
