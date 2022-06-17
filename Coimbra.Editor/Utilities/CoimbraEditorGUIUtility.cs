using CoimbraInternal.Editor;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Coimbra.Editor
{
    /// <summary>
    /// General editor GUI utilities.
    /// </summary>
    public static class CoimbraEditorGUIUtility
    {
        private const string RenderPipelineComponentWarningFormat = "The active render pipeline does not support the {0} component.";

        private static readonly char[] DefaultSearchSeparator =
        {
            ' '
        };

        private static readonly Dictionary<int, ReorderableList> ReorderableLists = new Dictionary<int, ReorderableList>();

        private static Dictionary<Type, PropertyDrawer> _propertyDrawers;

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
        /// Unified way to draw a property field using its specific <see cref="PropertyDrawer"/> even if it has a <see cref="PropertyAttribute"/> with another drawer.
        /// </summary>
        public static void DrawPropertyField(Rect position, SerializedProperty property, GUIContent label)
        {
            InitializePropertyDrawers();

            Type type = property.GetPropertyType();

            if (type.IsGenericType)
            {
                type = type.GetGenericTypeDefinition();
            }

            if (_propertyDrawers.TryGetValue(type, out PropertyDrawer propertyDrawer))
            {
                propertyDrawer.OnGUI(position, property, label);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
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
            using (GUIContentPool.Pop(out GUIContent content))
            {
                content.text = message;

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

                Rect rect = new Rect(0, 0, contentWidth, 0);
                AdjustPosition(ref rect, area);

                float height = EditorStyles.helpBox.CalcHeight(content, rect.width);

                return Mathf.Max(height, minContentHeight);
            }
        }

        /// <summary>
        /// Unified way to get a property height using its specific <see cref="PropertyDrawer"/> even if it has a <see cref="PropertyAttribute"/> with another drawer.
        /// </summary>
        public static float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            InitializePropertyDrawers();

            Type type = property.GetPropertyType();

            if (type.IsGenericType)
            {
                type = type.GetGenericTypeDefinition();
            }

            return _propertyDrawers.TryGetValue(type, out PropertyDrawer propertyDrawer)
                       ? propertyDrawer.GetPropertyHeight(property, label)
                       : EditorGUI.GetPropertyHeight(property, label);
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

            using (StringBuilderPool.Pop(out StringBuilder stringBuilder))
            {
                stringBuilder.EnsureCapacity(value.Length * 2);

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

                    Debug.LogWarning($"Invalid char {currentInput} on {value}! The only supported chars are digits, letters and underscore.");

                    currentInput = lastInput;
                }

                return stringBuilder.ToString();
            }
        }

        /// <summary>
        /// Basic way to check if if a <paramref name="searchContext"/> matches the desired the <paramref name="targetContent"/>.
        /// </summary>
        public static bool TryMatchSearch(string searchContext, string targetContent)
        {
            if (string.IsNullOrWhiteSpace(searchContext))
            {
                return true;
            }

            string[] split = searchContext.Split(DefaultSearchSeparator, StringSplitOptions.RemoveEmptyEntries);

            foreach (string value in split)
            {
                if (targetContent.IndexOf(value, StringComparison.InvariantCultureIgnoreCase) > -1)
                {
                    return true;
                }
            }

            return false;
        }

        internal static void DrawComponentWarningForRenderPipeline(Type type)
        {
            EditorGUILayout.HelpBox(string.Format(RenderPipelineComponentWarningFormat, type.Name), MessageType.Warning);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetPersistentHashCode(this PropertyModification propertyModification, bool isInstanceSpecific)
        {
            return GetPersistentHashCode(propertyModification.target, propertyModification.propertyPath, isInstanceSpecific);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetPersistentHashCode(this SerializedProperty serializedProperty, bool isInstanceSpecific)
        {
            return GetPersistentHashCode(serializedProperty.serializedObject.targetObject, serializedProperty.propertyPath, isInstanceSpecific);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsReorderableList(this PropertyModification propertyModification, out ReorderableList reorderableList)
        {
            return ReorderableLists.TryGetValue(propertyModification.GetPersistentHashCode(true), out reorderableList);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ReorderableList ToReorderableList(this SerializedProperty serializedProperty, Action<ReorderableList> onInitialize)
        {
            int persistentHashCode = serializedProperty.GetPersistentHashCode(true);

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
        private static int GetPersistentHashCode(Object target, string propertyPath, bool isInstanceSpecific)
        {
            int targetHash = isInstanceSpecific ? target.GetHashCode() : target.GetType().GetHashCode();
            int propertyPathHash = propertyPath.GetHashCode();

#if UNITY_2021_3_OR_NEWER
            return HashCode.Combine(targetHash, propertyPathHash);
#else
            return (targetHash, propertyPathHash).GetHashCode();
#endif
        }

        private static void InitializePropertyDrawers()
        {
            if (_propertyDrawers != null)
            {
                return;
            }

            _propertyDrawers = new Dictionary<Type, PropertyDrawer>();

            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            foreach (Type type in TypeCache.GetTypesWithAttribute<CustomPropertyDrawer>())
            {
                PropertyDrawer propertyDrawer;

                try
                {
                    propertyDrawer = (PropertyDrawer)Activator.CreateInstance(type);
                }
                catch
                {
                    try
                    {
                        propertyDrawer = (PropertyDrawer)type.GetConstructor(bindingFlags, null, Type.EmptyTypes, null)!.Invoke(null);
                    }
                    catch
                    {
                        continue;
                    }
                }

                foreach (CustomPropertyDrawer attribute in type.GetCustomAttributes<CustomPropertyDrawer>(true))
                {
                    _propertyDrawers[attribute.GetTargetType()] = propertyDrawer;

                    if (!attribute.GetUseForChildren())
                    {
                        continue;
                    }

                    foreach (Type derivedType in TypeCache.GetTypesDerivedFrom(attribute.GetTargetType()))
                    {
                        if (!_propertyDrawers.ContainsKey(derivedType))
                        {
                            _propertyDrawers.Add(derivedType, propertyDrawer);
                        }
                    }
                }
            }
        }
    }
}
