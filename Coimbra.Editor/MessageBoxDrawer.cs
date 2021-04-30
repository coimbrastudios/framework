using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    [CustomPropertyDrawer(typeof(MessageBoxAttribute))]
    [CustomPropertyDrawer(typeof(MessageBoxOnEditModeAttribute))]
    [CustomPropertyDrawer(typeof(MessageBoxOnPlayModeAttribute))]
    public sealed class MessageBoxDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            switch (attribute)
            {
                case MessageBoxOnEditModeAttribute _ when !CSFrameworkUtility.IsEditMode:
                case MessageBoxOnPlayModeAttribute _ when !CSFrameworkUtility.IsPlayMode:
                    return 0;

                default:
                {
                    MessageBoxAttribute messageBoxAttribute = (MessageBoxAttribute)attribute;

                    return CSFrameworkEditorGUIUtility.GetMessageBoxHeight(messageBoxAttribute.Message, messageBoxAttribute.Type, messageBoxAttribute.FillLabelArea, base.GetHeight());
                }
            }
        }

        public override void OnGUI(Rect position)
        {
            if (position.height == 0)
            {
                return;
            }

            MessageBoxAttribute messageBoxAttribute = (MessageBoxAttribute)attribute;
            CSFrameworkEditorGUIUtility.DrawMessageBox(position, messageBoxAttribute.Message, messageBoxAttribute.Type, messageBoxAttribute.FillLabelArea);
        }
    }
}
