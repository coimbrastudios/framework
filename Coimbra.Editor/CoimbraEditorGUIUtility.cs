using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// General editor GUI utilities.
    /// </summary>
    public static class CoimbraEditorGUIUtility
    {
        private static readonly Dictionary<int, ReorderableList> ReorderableLists = new();

        /// <summary>
        /// Adjust a position based on the specified <see cref="InspectorArea"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AdjustPosition(ref Rect position, InspectorArea area)
        {
            switch (area)
            {
                case InspectorArea.Field:
                {
                    position.xMin += EditorGUIUtility.labelWidth;

                    break;
                }

                case InspectorArea.Label:
                {
                    position.width = EditorGUIUtility.labelWidth;

                    break;
                }
            }
        }

        /// <summary>
        /// Draw a message box with the option to ignore the label area.
        /// </summary>
        /// <param name="position">Rectangle on the screen to draw the help box within.</param>
        /// <param name="message">The message text.</param>
        /// <param name="type">The type of message.</param>
        /// <param name="area">To use with <see cref="AdjustPosition"/>.</param>
        public static void DrawMessageBox(Rect position, string message, MessageBoxType type, InspectorArea area)
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

            AdjustPosition(ref position, area);
            EditorGUI.HelpBox(position, message, messageType);
        }

        /// <summary>
        /// Get the required height to draw a message box with a given message and type.
        /// </summary>
        /// <param name="message">The message text.</param>
        /// <param name="type">The type of message.</param>
        /// <param name="area">The inspector area to be used.</param>
        /// <param name="defaultMinContentHeight">The default min content height when no icon is present.</param>
        /// <returns></returns>
        public static float GetMessageBoxHeight(string message, MessageBoxType type, InspectorArea area, float defaultMinContentHeight)
        {
            GUIContent content = new(message);
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

            Rect rect = new(0, 0, contentWidth, 0);
            AdjustPosition(ref rect, area);

            float height = EditorStyles.helpBox.CalcHeight(content, rect.width);

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

            const string startBackingField = "<";
            const string endBackingField = ">k__BackingField";

            if (value.StartsWith(startBackingField) && value.EndsWith(endBackingField))
            {
                value = value.Substring(1, value.Length - startBackingField.Length - endBackingField.Length);
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

            StringBuilder stringBuilder = new(value.Length * 2);

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetPersistentHashCode(this PropertyModification propertyModification)
        {
            return GetPersistentHashCode(propertyModification.target, propertyModification.propertyPath);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetPersistentHashCode(this SerializedProperty serializedProperty)
        {
            return GetPersistentHashCode(serializedProperty.serializedObject.targetObject, serializedProperty.propertyPath);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsReorderableList(this PropertyModification propertyModification, out ReorderableList reorderableList)
        {
            return ReorderableLists.TryGetValue(propertyModification.GetPersistentHashCode(), out reorderableList);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ReorderableList ToReorderableList(this SerializedProperty serializedProperty, Action<ReorderableList> onInitialize)
        {
            int persistentHashCode = serializedProperty.GetPersistentHashCode();

            if (ReorderableLists.TryGetValue(persistentHashCode, out ReorderableList reorderableList))
            {
                reorderableList.serializedProperty = serializedProperty;

                return reorderableList;
            }

            reorderableList = new ReorderableList(serializedProperty.serializedObject, serializedProperty);
            ReorderableLists.Add(persistentHashCode, reorderableList);
            onInitialize?.Invoke(reorderableList);

            return reorderableList;
        }

        private static void FitIcon(string icon, ref float contentWidth, out float minContentHeight)
        {
            GUIContent iconContent = EditorGUIUtility.IconContent(icon);
            contentWidth -= iconContent.image.width;
            minContentHeight = iconContent.image.height;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetPersistentHashCode(UnityEngine.Object target, string propertyPath)
        {
            int targetHash = target.GetHashCode();
            int propertyPathHash = propertyPath.GetHashCode();

            return HashCode.Combine(targetHash, propertyPathHash);
        }
    }
}
