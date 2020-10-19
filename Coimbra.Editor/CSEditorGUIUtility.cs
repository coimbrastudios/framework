using System.Text;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    internal static class CSEditorGUIUtility
    {
        internal static void DrawMessageBox(Rect position, string message, MessageType type, bool fillLabelArea)
        {
            UnityEditor.MessageType messageType;

            switch (type)
            {
                case MessageType.Info:
                {
                    messageType = UnityEditor.MessageType.Info;

                    break;
                }

                case MessageType.Warning:
                {
                    messageType = UnityEditor.MessageType.Warning;

                    break;
                }

                case MessageType.Error:
                {
                    messageType = UnityEditor.MessageType.Error;

                    break;
                }

                default:
                {
                    messageType = UnityEditor.MessageType.None;

                    break;
                }
            }

            if (!fillLabelArea)
            {
                position.xMin += EditorGUIUtility.labelWidth;
            }

            EditorGUI.HelpBox(position, message, messageType);
        }

        internal static float GetMessageBoxHeight(string message, MessageType type, bool fillLabelArea, float defaultHeight)
        {
            GUIContent content = new GUIContent(message);
            float contentWidth = EditorGUIUtility.currentViewWidth - EditorStyles.foldout.CalcSize(GUIContent.none).x - EditorStyles.inspectorDefaultMargins.padding.horizontal;
            float minContentHeight;

            switch (type)
            {
                case MessageType.Info:
                {
                    // ReSharper disable once StringLiteralTypo
                    FitIcon("console.infoicon", ref contentWidth, out minContentHeight);

                    break;
                }

                case MessageType.Warning:
                {
                    // ReSharper disable once StringLiteralTypo
                    FitIcon("console.warnicon", ref contentWidth, out minContentHeight);

                    break;
                }

                case MessageType.Error:
                {
                    // ReSharper disable once StringLiteralTypo
                    FitIcon("console.erroricon", ref contentWidth, out minContentHeight);

                    break;
                }

                default:
                {
                    minContentHeight = defaultHeight;

                    break;
                }
            }

            if (!fillLabelArea)
            {
                contentWidth -= EditorGUIUtility.labelWidth;
            }

            float height = EditorStyles.helpBox.CalcHeight(content, contentWidth);

            return Mathf.Max(height, minContentHeight);
        }

        internal static string ToDisplayName(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return s;
            }

            StringBuilder stringBuilder = new StringBuilder(s.Length * 2);
            stringBuilder.Append(s[0]);

            for (int i = 1; i < s.Length; i++)
            {
                if (char.IsLower(s, i))
                {
                    stringBuilder.Append(s[i]);

                    continue;
                }

                if (char.IsLower(s, i - 1))
                {
                    stringBuilder.Append(" ");
                    stringBuilder.Append(s[i]);

                    continue;
                }

                if (i + 1 < s.Length && char.IsLower(s, i + 1))
                {
                    stringBuilder.Append(" ");
                    stringBuilder.Append(s[i]);

                    continue;
                }

                stringBuilder.Append(s[i]);
            }

            return stringBuilder.ToString();
        }

        private static void FitIcon(string icon, ref float contentWidth, out float minContentHeight)
        {
            GUIContent iconContent = EditorGUIUtility.IconContent(icon);
            contentWidth -= iconContent.image.width;
            minContentHeight = iconContent.image.height;
        }
    }
}
