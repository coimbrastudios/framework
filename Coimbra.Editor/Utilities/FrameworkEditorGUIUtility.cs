using System.Text;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// General editor GUI utilities.
    /// </summary>
    public static class FrameworkEditorGUIUtility
    {
        /// <summary>
        /// Draw a message box with the option to ignore the label area.
        /// </summary>
        /// <param name="position">Rectangle on the screen to draw the help box within.</param>
        /// <param name="message">The message text.</param>
        /// <param name="type">The type of message.</param>
        /// <param name="fillLabelArea">If false, the label area will be skipped.</param>
        public static void DrawMessageBox(Rect position, string message, MessageBoxType type, bool fillLabelArea)
        {
            MessageType messageType;

            switch (type)
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

            if (!fillLabelArea)
            {
                position.xMin += EditorGUIUtility.labelWidth;
            }

            EditorGUI.HelpBox(position, message, messageType);
        }

        /// <summary>
        /// Get the required height to draw a message box with a given message and type.
        /// </summary>
        /// <param name="message">The message text.</param>
        /// <param name="type">The type of message.</param>
        /// <param name="fillLabelArea">If false, the label area will be skipped.</param>
        /// <param name="defaultMinContentHeight">The default min content height when no icon is present.</param>
        /// <returns></returns>
        public static float GetMessageBoxHeight(string message, MessageBoxType type, bool fillLabelArea, float defaultMinContentHeight)
        {
            GUIContent content = new GUIContent(message);
            float contentWidth = EditorGUIUtility.currentViewWidth - EditorStyles.foldout.CalcSize(GUIContent.none).x - EditorStyles.inspectorDefaultMargins.padding.horizontal;
            float minContentHeight;

            switch (type)
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
                    minContentHeight = defaultMinContentHeight;

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

        /// <summary>
        /// Create a more human-readable string from the input value. Ex: CSEditorGUIUtility turns into CS Editor GUI Utility.
        /// </summary>
        /// <param name="value">The input value.</param>
        /// <returns>The more human-readable string.</returns>
        public static string ToDisplayName(string value)
        {
            const char underscore = '_';

            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            int i = 0;

            if (value.Length > 1 && value[1] == underscore)
            {
                i += 2;
            }

            if (value.Length <= i)
            {
                return string.Empty;
            }

            while (value[i] == underscore)
            {
                i++;

                if (value.Length == i)
                {
                    return string.Empty;
                }
            }

            StringBuilder stringBuilder = new StringBuilder(value.Length * 2);

            char currentInput = value[i];
            char lastOutput = char.ToUpper(currentInput);
            stringBuilder.Append(lastOutput);
            i++;

            int underscoreSequence = 0;
            int letterSequence = char.IsNumber(lastOutput) ? 0 : 1;

            for (; i < value.Length; i++)
            {
                char lastInput = currentInput;
                currentInput = value[i];

                if (currentInput == underscore)
                {
                    letterSequence = 0;
                    underscoreSequence++;

                    continue;
                }

                bool hasUnderscoreSequence = underscoreSequence > 1;
                underscoreSequence = 0;

                if (char.IsNumber(currentInput))
                {
                    letterSequence = 0;

                    if (char.IsNumber(lastOutput))
                    {
                        if (lastInput == underscore)
                        {
                            stringBuilder.Append(hasUnderscoreSequence ? ' ' : '.');
                        }
                    }
                    else
                    {
                        stringBuilder.Append(' ');
                    }

                    lastOutput = currentInput;
                    stringBuilder.Append(lastOutput);

                    continue;
                }

                if (char.IsUpper(currentInput))
                {
                    if (char.IsNumber(lastOutput) || char.IsLower(lastOutput))
                    {
                        stringBuilder.Append(' ');
                    }
                    else if (char.IsUpper(lastOutput) && i + 1 < value.Length && char.IsLower(value[i + 1]))
                    {
                        stringBuilder.Append(' ');
                    }

                    lastOutput = currentInput;
                    stringBuilder.Append(lastOutput);
                    letterSequence++;

                    continue;
                }

                if (char.IsLower(currentInput))
                {
                    if (char.IsNumber(lastOutput) || lastInput == underscore)
                    {
                        lastOutput = char.ToUpper(currentInput);
                        stringBuilder.Append(' ');
                        stringBuilder.Append(lastOutput);
                        letterSequence++;

                        continue;
                    }

                    if (char.IsLower(lastOutput))
                    {
                        lastOutput = currentInput;
                        stringBuilder.Append(lastOutput);
                        letterSequence++;

                        continue;
                    }

                    if (letterSequence == 0)
                    {
                        lastOutput = char.ToUpper(currentInput);
                        stringBuilder.Append(lastOutput);
                    }
                    else
                    {
                        lastOutput = currentInput;
                        stringBuilder.Append(lastOutput);
                    }

                    letterSequence++;

                    continue;
                }

                Debug.LogWarning($"Invalid char {currentInput}! The only supported chars are digits, letters and underscore.");

                currentInput = lastInput;
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
