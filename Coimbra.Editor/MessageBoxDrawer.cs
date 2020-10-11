using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    [CustomPropertyDrawer(typeof(MessageBoxAttribute))]
    public sealed class MessageBoxDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            MessageBoxAttribute messageBoxAttribute = (MessageBoxAttribute)attribute;
            GUIContent content = new GUIContent(messageBoxAttribute.Message);
            float contentWidth = EditorGUIUtility.currentViewWidth - EditorStyles.foldout.CalcSize(GUIContent.none).x - EditorStyles.inspectorDefaultMargins.padding.horizontal;
            float minContentHeight;

            switch (messageBoxAttribute.Type)
            {
                case MessageBoxType.Info:
                {
                    // ReSharper disable once StringLiteralTypo
                    FitIcon("console.infoicon", ref contentWidth, out minContentHeight);

                    break;
                }

                case MessageBoxType.Warning:
                {
                    // ReSharper disable once StringLiteralTypo
                    FitIcon("console.warnicon", ref contentWidth, out minContentHeight);

                    break;
                }

                case MessageBoxType.Error:
                {
                    // ReSharper disable once StringLiteralTypo
                    FitIcon("console.erroricon", ref contentWidth, out minContentHeight);

                    break;
                }

                default:
                {
                    minContentHeight = base.GetHeight();

                    break;
                }
            }

            float height = EditorStyles.helpBox.CalcHeight(content, contentWidth);

            return Mathf.Max(height, minContentHeight);
        }

        public override void OnGUI(Rect position)
        {
            MessageBoxAttribute messageBoxAttribute = (MessageBoxAttribute)attribute;
            MessageType messageType;

            switch (messageBoxAttribute.Type)
            {
                case MessageBoxType.Info:
                {
                    messageType = MessageType.Info;

                    break;
                }

                case MessageBoxType.Warning:
                {
                    messageType = MessageType.Warning;

                    break;
                }

                case MessageBoxType.Error:
                {
                    messageType = MessageType.Error;

                    break;
                }

                default:
                {
                    messageType = MessageType.None;

                    break;
                }
            }

            EditorGUI.HelpBox(position, messageBoxAttribute.Message, messageType);
        }

        private void FitIcon(string icon, ref float contentWidth, out float minContentHeight)
        {
            GUIContent iconContent = EditorGUIUtility.IconContent(icon);
            contentWidth -= iconContent.image.width;
            minContentHeight = iconContent.image.height;
        }
    }
}
