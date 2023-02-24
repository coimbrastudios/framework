using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Drawer for <see cref="MessageBoxAttributeBase"/> and its inheritors.
    /// </summary>
    [CustomPropertyDrawer(typeof(MessageBoxAttributeBase), true)]
    public sealed class MessageBoxDrawer : DecoratorDrawer
    {
        /// <inheritdoc/>
        public override float GetHeight()
        {
            if (attribute is MessageBoxAttributeBase messageBoxAttribute && messageBoxAttribute.ShouldDisplayMessageBox())
            {
                return EngineUtility.GetMessageBoxHeight(messageBoxAttribute.Message, messageBoxAttribute.Type, messageBoxAttribute.Area, base.GetHeight());
            }

            return 0;
        }

        /// <inheritdoc/>
        public override void OnGUI(Rect position)
        {
            if (attribute is MessageBoxAttributeBase messageBoxAttribute && messageBoxAttribute.ShouldDisplayMessageBox())
            {
                EngineUtility.DrawMessageBox(position, messageBoxAttribute.Message, messageBoxAttribute.Type, messageBoxAttribute.Area);
            }
        }
    }
}
