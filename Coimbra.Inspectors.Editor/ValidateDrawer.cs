#nullable enable

using UnityEditor;

namespace Coimbra.Inspectors.Editor
{
    /// <summary>
    /// Drawer for <see cref="ValidateAttribute"/>.
    /// </summary>
    [InspectorDecoratorDrawer(typeof(ValidateAttribute))]
    public sealed class ValidateDrawer : IInspectorDecoratorDrawer
    {
        /// <inheritdoc/>
        public float GetHeightAfterGUI(ref InspectorDecoratorDrawerContext context)
        {
            return 0;
        }

        /// <inheritdoc/>
        public float GetHeightBeforeGUI(ref InspectorDecoratorDrawerContext context)
        {
            return 0;
        }

        /// <inheritdoc/>
        public void OnAfterGUI(ref InspectorDecoratorDrawerContext context)
        {
            if (!EditorGUI.EndChangeCheck())
            {
                return;
            }

            // TODO
        }

        public void OnBeforeGUI(ref InspectorDecoratorDrawerContext context)
        {
            EditorGUI.BeginChangeCheck();
        }
    }
}
