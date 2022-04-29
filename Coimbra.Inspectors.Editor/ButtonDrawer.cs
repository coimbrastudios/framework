#nullable enable

using Coimbra.Editor;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Inspectors.Editor
{
    /// <summary>
    /// Drawer for <see cref="ButtonAttribute"/>.
    /// </summary>
    [InspectorDecoratorDrawer(typeof(ButtonAttribute), true)]
    public sealed class ButtonDrawer : IInspectorDecoratorDrawer
    {
        private const string InvalidMethodMessageFormat = "Invalid method \"{0}\"";

        private const string InvalidSignatureMessageFormat = "Invalid signature, expected \"{0}():void\"";

        private const string MethodIsNullOrWhiteSpaceMessage = "Method is null or white space.";

        /// <inheritdoc/>
        public float GetHeightAfterGUI(ref InspectorDecoratorDrawerContext context)
        {
            ButtonAttribute buttonAttribute = (ButtonAttribute)context.Attribute;

            return buttonAttribute.Position == DecoratorPosition.AfterGUI ? GetHeight(buttonAttribute) : 0;
        }

        /// <inheritdoc/>
        public float GetHeightBeforeGUI(ref InspectorDecoratorDrawerContext context)
        {
            ButtonAttribute buttonAttribute = (ButtonAttribute)context.Attribute;

            return buttonAttribute.Position == DecoratorPosition.BeforeGUI ? GetHeight(buttonAttribute) : 0;
        }

        /// <inheritdoc/>
        public void OnAfterGUI(ref InspectorDecoratorDrawerContext context)
        {
            OnGUI(ref context);
        }

        /// <inheritdoc/>
        public void OnBeforeGUI(ref InspectorDecoratorDrawerContext context)
        {
            OnGUI(ref context);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DrawButton(Rect position, string method, GUIStyle style, IReadOnlyList<object> scopes)
        {
            string displayName = CoimbraEditorGUIUtility.ToDisplayName(method);

            if (!GUI.Button(position, displayName, style))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(method))
            {
                Debug.LogError(MethodIsNullOrWhiteSpaceMessage);

                return;
            }

            MethodInfo? methodInfo = scopes[0].GetType().GetMethodSlow(method, true);

            if (methodInfo == null)
            {
                Debug.LogErrorFormat(InvalidMethodMessageFormat, method);

                return;
            }

            if (methodInfo.GetParameters().Length != 0)
            {
                Debug.LogErrorFormat(InvalidSignatureMessageFormat, method);

                return;
            }

            if (methodInfo.IsStatic)
            {
                methodInfo.Invoke(null, null);
            }
            else
            {
                for (int i = 0; i < scopes.Count; i++)
                {
                    methodInfo.Invoke(scopes[i], null);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float GetHeight(ButtonAttribute buttonAttribute)
        {
            return InspectorUtility.CheckConditions(buttonAttribute.Predicate, buttonAttribute.Conditions) && buttonAttribute.Methods.Length > 0 ? EditorGUIUtility.singleLineHeight : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void OnGUI(ref InspectorDecoratorDrawerContext context)
        {
            if (context.Position.height == 0)
            {
                return;
            }

            Rect position = context.Position;
            ButtonAttribute buttonAttribute = (ButtonAttribute)context.Attribute;
            CoimbraEditorGUIUtility.AdjustPosition(ref position, buttonAttribute.Area);

            if (buttonAttribute.Methods.Length == 1)
            {
                DrawButton(position, buttonAttribute.Methods[0], EditorStyles.miniButton, context.Scopes);

                return;
            }

            int last = buttonAttribute.Methods.Length - 1;
            float width = position.width / buttonAttribute.Methods.Length;
            position.width = width;
            DrawButton(position, buttonAttribute.Methods[0], EditorStyles.miniButtonLeft, context.Scopes);

            for (int i = 1; i < last; i++)
            {
                position.x += width;
                DrawButton(position, buttonAttribute.Methods[i], EditorStyles.miniButtonMid, context.Scopes);
            }

            position.x += width;
            DrawButton(position, buttonAttribute.Methods[last], EditorStyles.miniButtonRight, context.Scopes);
        }
    }
}
