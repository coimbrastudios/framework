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
                case MessageBoxOnEditModeAttribute _ when !CSUtility.IsEditMode:
                case MessageBoxOnPlayModeAttribute _ when !CSUtility.IsPlayMode:
                    return 0;

                default:
                {
                    MessageBoxAttribute messageBoxAttribute = (MessageBoxAttribute)attribute;

                    return CSEditorGUIUtility.GetMessageBoxHeight(messageBoxAttribute.Message, messageBoxAttribute.Type, messageBoxAttribute.FillLabelArea, base.GetHeight());
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
            CSEditorGUIUtility.DrawMessageBox(position, messageBoxAttribute.Message, messageBoxAttribute.Type, messageBoxAttribute.FillLabelArea);
        }
    }
}
