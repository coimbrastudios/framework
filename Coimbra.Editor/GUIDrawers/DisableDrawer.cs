using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Drawer for <see cref="DisableAttributeBase"/> and its inheritors.
    /// </summary>
    [CustomPropertyDrawer(typeof(DisableAttributeBase), true)]
    public sealed class DisableDrawer : DecoratorDrawer
    {
        /// <inheritdoc/>
        public override float GetHeight()
        {
            return 0;
        }

        /// <inheritdoc/>
        public override void OnGUI(Rect position)
        {
            if (attribute is DisableAttributeBase disableAttribute && disableAttribute.ShouldDisableGUI())
            {
                GUI.enabled = false;
            }
        }
    }
}
