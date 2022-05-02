#nullable enable

using UnityEditor;
using UnityEngine;

namespace Coimbra.Inspectors.Editor
{
    /// <summary>
    /// Drawer for <see cref="ValidateAttribute"/>.
    /// </summary>
    [InspectorDecoratorDrawer(typeof(ValidateAttribute))]
    public sealed class ValidateDrawer : IInspectorDecoratorDrawer
    {
        /// <inheritdoc/>
        public float GetAfterGUIHeight(ref InspectorDecoratorDrawerContext context)
        {
            return 0;
        }

        /// <inheritdoc/>
        public float GetBeforeGUIHeight(ref InspectorDecoratorDrawerContext context)
        {
            return 0;
        }

        /// <inheritdoc/>
        public void OnAfterGUI(Rect position, ref InspectorDecoratorDrawerContext context)
        {
            if (!EditorGUI.EndChangeCheck())
            {
                return;
            }

            // TODO
        }

        public void OnBeforeGUI(Rect position, ref InspectorDecoratorDrawerContext context)
        {
            EditorGUI.BeginChangeCheck();
        }
    }
}
