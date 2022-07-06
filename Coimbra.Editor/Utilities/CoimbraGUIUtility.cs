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
    public static class CoimbraGUIUtility
    {
        /// <summary>
        /// Gets the default count field size for any list or array.
        /// </summary>
        public const int ListCountFieldSize = 50;

        private const string RenderPipelineComponentWarningFormat = "The active render pipeline does not support the {0} component.";

        private static readonly char[] DefaultSearchSeparator =
        {
            ' ',
        };

        private static readonly Dictionary<int, ReorderableList> ReorderableLists = new();

        private static Dictionary<Type, PropertyDrawer> _propertyDrawers;

        private static ReorderableList _delegateListeners;

        /// <summary>
        /// Gets a position based on the specified <see cref="InspectorArea"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect AdjustPosition(Rect position, InspectorArea area)
        {
            Rect result = position;
            AdjustPosition(ref result, area);

            return result;
        }

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
                    position.xMin += EditorGUIUtility.labelWidth + 2;

                    break;
                }

                case InspectorArea.Label:
                {
                    position.width = EditorGUIUtility.labelWidth - UnityEditorInternals.GetIndent();

                    break;
                }
            }
        }

        /// <summary>
        /// Draws a delegate invocation list.
        /// </summary>
        /// <param name="position">The position to draw.</param>
        /// <param name="label">The label to draw.</param>
        /// <param name="list">The list to draw.</param>
        /// <param name="isMultiEditing">If true, a warning will appear instead of the actual invocation list.</param>
        public static void DrawDelegateListeners(Rect position, GUIContent label, List<DelegateListener> list, bool isMultiEditing)
        {
            InitializeDelegateListeners();
            position = EditorGUI.IndentedRect(position);

            using (new ResetIndentLevelScope())
            {
                Rect headerPosition = position;
                headerPosition.height = EditorGUIUtility.singleLineHeight;

                if (!isMultiEditing)
                {
                    _delegateListeners.list = list;
                }

                _delegateListeners.DoList(position);

                _delegateListeners.list = null;
                headerPosition.xMin += 6f;
                headerPosition.height -= 2f;
                headerPosition.y++;
                EditorGUI.LabelField(headerPosition, label);

                headerPosition.xMin += headerPosition.width - ListCountFieldSize;
                EditorGUI.IntField(headerPosition, list.Count);
            }
        }

        /// <summary>
        /// Draws a delegate invocation list.
        /// </summary>
        /// <param name="position">The position to draw.</param>
        /// <param name="label">The label to draw.</param>
        /// <param name="value">The delegate value to get the listeners to draw.</param>
        /// <param name="isMultiEditing">If true, a warning will appear instead of the actual invocation list.</param>
        public static void DrawDelegateListeners<T>(Rect position, GUIContent label, in T value, bool isMultiEditing)
            where T : Delegate
        {
            using (ListPool.Pop(out List<DelegateListener> list))
            {
                value.GetListeners(list);
                DrawDelegateListeners(position, label, list, isMultiEditing);
            }
        }

        /// <summary>
        /// Draw a message box with the option to ignore the label area.
        /// </summary>
        /// <param name="position">Rectangle on the screen to draw the help box within.</param>
        /// <param name="message">The message text.</param>
        /// <param name="type">The type of message.</param>
        /// <param name="area">Where to drawn.</param>
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
#pragma warning disable UNT0027
                propertyDrawer.OnGUI(position, property, label);
#pragma warning restore UNT0027
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }

        /// <summary>
        /// Gets the necessary height to draw a list of listeners in the inspector.
        /// </summary>
        public static float GetDelegateListenersHeight<T>(in T value, bool isMultiEditing)
            where T : Delegate
        {
            using (ListPool.Pop(out List<DelegateListener> list))
            {
                value.GetListeners(list);

                return GetDelegateListenersHeight(list, isMultiEditing);
            }
        }

        /// <summary>
        /// Gets the necessary height to draw a list of listeners in the inspector.
        /// </summary>
        public static float GetDelegateListenersHeight(List<DelegateListener> list, bool isMultiEditing)
        {
            InitializeDelegateListeners();

            if (isMultiEditing)
            {
                return _delegateListeners.GetHeight();
            }

            _delegateListeners.list = list;

            float result = _delegateListeners.GetHeight();
            _delegateListeners.list = null;

            return result;
        }

        /// <summary>
        /// Create a more human-readable string from the input value. Ex: CSEditorGUIUtility turns into CS Editor GUI Utility.
        /// </summary>
        /// <param name="value">The input value.</param>
        /// <returns>The more human-readable string.</returns>
        public static string GetDisplayName(string value)
        {
            if (IsNullOrUnderscores(ref value, out int i))
            {
                return string.Empty;
            }

            const char underscore = '_';

            using (StringBuilderPool.Pop(out StringBuilder stringBuilder))
            {
                stringBuilder.EnsureCapacity(value.Length * 2);

                char currentInput = value[i];
                char lastOutput = char.ToUpper(currentInput);
                int underscoreSequence = 0;
                int letterSequence = char.IsNumber(lastOutput) ? 0 : 1;
                stringBuilder.Append(lastOutput);

                for (i++; i < value.Length; i++)
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

                    if (TryAppendNumber(stringBuilder, currentInput, ref lastOutput, ref letterSequence, lastInput, hasUnderscoreSequence, underscore))
                    {
                        continue;
                    }

                    if (TryAppendUpper(stringBuilder, currentInput, ref lastOutput, ref letterSequence, in value, i))
                    {
                        continue;
                    }

                    if (TryAppendLower(stringBuilder, currentInput, ref lastOutput, ref letterSequence, lastInput, underscore))
                    {
                        continue;
                    }

                    // ignore unsupported char
                    currentInput = lastInput;
                }

                return stringBuilder.ToString();
            }
        }

        /// <summary>
        /// Get the required height to draw a message box with a given message and type.
        /// </summary>
        /// <param name="message">The message text.</param>
        /// <param name="type">The type of message.</param>
        /// <param name="area">The inspector area to be used.</param>
        /// <param name="defaultMinContentHeight">The default min content height when no icon is present.</param>
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

                Rect rect = new(0, 0, contentWidth, 0);
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
        /// Basic way to check if a <paramref name="searchContext"/> matches the desired the <paramref name="targetContent"/>.
        /// </summary>
        public static bool TryMatchSearch(in string searchContext, in string targetContent)
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

        internal static int DrawListHeader(Rect position, GUIContent label, SerializedProperty property, ReorderableList list)
        {
            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, property, label, false);

            using (new EditorGUI.DisabledScope(true))
            using (new ResetIndentLevelScope())
            {
                position.xMin += position.width - ListCountFieldSize;

                using (new ShowMixedValueScope(list.serializedProperty.HasMultipleDifferentArraySizes()))
                {
                    return EditorGUI.IntField(position, list.serializedProperty.arraySize);
                }
            }
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

        private static void InitializeDelegateListeners()
        {
            if (_delegateListeners != null)
            {
                return;
            }

            _delegateListeners = new ReorderableList(null, typeof(DelegateListener), false, true, false, false)
            {
                headerHeight = EditorGUIUtility.singleLineHeight,
                elementHeight = EditorGUIUtility.singleLineHeight,
                footerHeight = 0,
                drawHeaderCallback = delegate
                {
                    // empty
                },
                onSelectCallback = delegate(ReorderableList list)
                {
                    list.index = -1;
                },
            };

            _delegateListeners.drawNoneElementCallback = delegate(Rect rect)
            {
                if (_delegateListeners.list != null)
                {
                    ReorderableList.defaultBehaviours.DrawNoneElement(rect, false);
                }
                else
                {
                    EditorGUI.LabelField(rect, "Can't inspect multiple objects!");
                }
            };

            _delegateListeners.drawElementCallback = delegate(Rect rect, int index, bool _, bool _)
            {
                if (_delegateListeners.serializedProperty != null)
                {
                    EditorGUI.PropertyField(rect, _delegateListeners.serializedProperty.GetArrayElementAtIndex(index));
                }
                else if (_delegateListeners.list != null)
                {
                    DelegateListener listener = (DelegateListener)_delegateListeners.list[index];
                    DelegateListenerDrawer.DrawGUI(rect, listener.Target, listener.Method, listener.IsStatic);
                }
            };
        }

        private static void InitializePropertyDrawers()
        {
            if (_propertyDrawers != null)
            {
                return;
            }

            _propertyDrawers = new Dictionary<Type, PropertyDrawer>();

            static void setPropertyDrawerForChildren(CustomPropertyDrawer attribute, PropertyDrawer propertyDrawer)
            {
                if (!attribute.GetUseForChildren())
                {
                    return;
                }

                foreach (Type derivedType in TypeCache.GetTypesDerivedFrom(attribute.GetTargetType()))
                {
                    if (!_propertyDrawers.ContainsKey(derivedType))
                    {
                        _propertyDrawers.Add(derivedType, propertyDrawer);
                    }
                }
            }

            foreach (Type type in TypeCache.GetTypesWithAttribute<CustomPropertyDrawer>())
            {
                if (!type.TryCreateInstance(out PropertyDrawer propertyDrawer))
                {
                    continue;
                }

                foreach (CustomPropertyDrawer attribute in type.GetCustomAttributes<CustomPropertyDrawer>(true))
                {
                    _propertyDrawers[attribute.GetTargetType()] = propertyDrawer;
                    setPropertyDrawerForChildren(attribute, propertyDrawer);
                }
            }
        }

        private static bool IsNullOrUnderscores(ref string value, out int firstIndexOfNonUnderscore)
        {
            const char underscore = '_';
            firstIndexOfNonUnderscore = 0;

            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }

            const string startBackingField = "<";
            const string endBackingField = ">k__BackingField";

            if (value.StartsWith(startBackingField) && value.EndsWith(endBackingField))
            {
                value = value.Substring(1, value.Length - startBackingField.Length - endBackingField.Length);
            }

            if (value.Length > 1 && value[1] == underscore)
            {
                firstIndexOfNonUnderscore += 2;
            }

            if (value.Length <= firstIndexOfNonUnderscore)
            {
                return true;
            }

            while (value[firstIndexOfNonUnderscore] == underscore)
            {
                firstIndexOfNonUnderscore++;

                if (value.Length != firstIndexOfNonUnderscore)
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        private static bool TryAppendLower(StringBuilder stringBuilder, char currentInput, ref char lastOutput, ref int letterSequence, char lastInput, char underscore)
        {
            if (!char.IsLower(currentInput))
            {
                return false;
            }

            if (char.IsNumber(lastOutput) || lastInput == underscore)
            {
                lastOutput = char.ToUpper(currentInput);
                stringBuilder.Append(' ');
                stringBuilder.Append(lastOutput);
                letterSequence++;

                return true;
            }

            if (char.IsLower(lastOutput))
            {
                lastOutput = currentInput;
                stringBuilder.Append(lastOutput);
                letterSequence++;

                return true;
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

            return true;
        }

        private static bool TryAppendNumber(StringBuilder stringBuilder, char currentInput, ref char lastOutput, ref int letterSequence, char lastInput, bool hasUnderscoreSequence, char underscore)
        {
            if (!char.IsNumber(currentInput))
            {
                return false;
            }

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

            return true;
        }

        private static bool TryAppendUpper(StringBuilder stringBuilder, char currentInput, ref char lastOutput, ref int letterSequence, in string rawInput, int currentIndex)
        {
            if (!char.IsUpper(currentInput))
            {
                return false;
            }

            if (char.IsNumber(lastOutput) || char.IsLower(lastOutput))
            {
                stringBuilder.Append(' ');
            }
            else if (char.IsUpper(lastOutput) && currentIndex + 1 < rawInput.Length && char.IsLower(rawInput[currentIndex + 1]))
            {
                stringBuilder.Append(' ');
            }

            lastOutput = currentInput;
            stringBuilder.Append(lastOutput);
            letterSequence++;

            return true;
        }
    }
}
