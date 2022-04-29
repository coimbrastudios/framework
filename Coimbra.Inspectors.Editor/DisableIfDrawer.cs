#nullable enable

using UnityEngine;

namespace Coimbra.Inspectors.Editor
{
    /// <summary>
    /// Drawer for <see cref="DisableIfAttribute"/>.
    /// </summary>
    [InspectorDecoratorDrawer(typeof(DisableIfAttribute))]
    public sealed class DisableIfDrawer : IInspectorDecoratorDrawer
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
        public void OnAfterGUI(ref InspectorDecoratorDrawerContext context) { }

        /// <inheritdoc/>
        public void OnBeforeGUI(ref InspectorDecoratorDrawerContext context)
        {
            DisableIfAttribute attribute = (DisableIfAttribute)context.Attribute;

            if (InspectorUtility.CheckConditions(attribute.Predicate, attribute.Conditions))
            {
                GUI.enabled = false;
            }
        }
    }
}
