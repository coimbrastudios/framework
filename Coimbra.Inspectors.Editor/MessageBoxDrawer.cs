#nullable enable

using Coimbra.Editor;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Inspectors.Editor
{
    /// <summary>
    /// Drawer for <see cref="MessageBoxAttribute"/>.
    /// </summary>
    [InspectorDecoratorDrawer(typeof(MessageBoxAttribute))]
    public sealed class MessageBoxDrawer : IInspectorDecoratorDrawer
    {
        /// <inheritdoc/>
        public float GetAfterGUIHeight(ref InspectorDecoratorDrawerContext context)
        {
            MessageBoxAttribute messageBoxAttribute = (MessageBoxAttribute)context.Attribute;

            return messageBoxAttribute.Position == DecoratorPosition.AfterGUI ? GetHeight(messageBoxAttribute) : 0;
        }

        /// <inheritdoc/>
        public float GetBeforeGUIHeight(ref InspectorDecoratorDrawerContext context)
        {
            MessageBoxAttribute messageBoxAttribute = (MessageBoxAttribute)context.Attribute;

            return messageBoxAttribute.Position == DecoratorPosition.BeforeGUI ? GetHeight(messageBoxAttribute) : 0;
        }

        /// <inheritdoc/>
        public void OnAfterGUI(Rect position, ref InspectorDecoratorDrawerContext context)
        {
            OnGUI(position, ref context);
        }

        /// <inheritdoc/>
        public void OnBeforeGUI(Rect position, ref InspectorDecoratorDrawerContext context)
        {
            OnGUI(position, ref context);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float GetHeight(MessageBoxAttribute messageBoxAttribute)
        {
            return CoimbraEditorGUIUtility.GetMessageBoxHeight(messageBoxAttribute.Message, messageBoxAttribute.Type, messageBoxAttribute.Area, EditorGUIUtility.singleLineHeight);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void OnGUI(Rect position, ref InspectorDecoratorDrawerContext context)
        {
            if (position.height == 0)
            {
                return;
            }

            MessageBoxAttribute messageBoxAttribute = (MessageBoxAttribute)context.Attribute;
            CoimbraEditorGUIUtility.DrawMessageBox(position, messageBoxAttribute.Message, messageBoxAttribute.Type, messageBoxAttribute.Area);
        }
    }
}
