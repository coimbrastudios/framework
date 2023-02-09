using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Drawer for <see cref="MessageBoxAttribute"/>, <see cref="MessageBoxOnEditModeAttribute"/>, and <see cref="MessageBoxOnPlayModeAttribute"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(MessageBoxAttribute))]
    [CustomPropertyDrawer(typeof(MessageBoxOnEditModeAttribute))]
    [CustomPropertyDrawer(typeof(MessageBoxOnPlayModeAttribute))]
    public sealed class MessageBoxDrawer : DecoratorDrawer
    {
        /// <inheritdoc/>
        public override float GetHeight()
        {
            switch (attribute)
            {
                case MessageBoxOnEditModeAttribute _ when !ApplicationUtility.IsEditMode:
                case MessageBoxOnPlayModeAttribute _ when !ApplicationUtility.IsPlayMode:
                    return 0;

                default:
                {
                    MessageBoxAttribute messageBoxAttribute = (MessageBoxAttribute)attribute;

                    return EngineUtility.GetMessageBoxHeight(messageBoxAttribute.Message, messageBoxAttribute.Type, messageBoxAttribute.Area, base.GetHeight());
                }
            }
        }

        /// <inheritdoc/>
        public override void OnGUI(Rect position)
        {
            if (position.height == 0)
            {
                return;
            }

            MessageBoxAttribute messageBoxAttribute = (MessageBoxAttribute)attribute;
            EngineUtility.DrawMessageBox(position, messageBoxAttribute.Message, messageBoxAttribute.Type, messageBoxAttribute.Area);
        }
    }
}
