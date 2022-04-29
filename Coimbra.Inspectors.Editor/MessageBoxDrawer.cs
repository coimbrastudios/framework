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
        public float GetHeightAfterGUI(ref InspectorDecoratorDrawerContext context)
        {
            MessageBoxAttribute messageBoxAttribute = (MessageBoxAttribute)context.Attribute;

            return messageBoxAttribute.Position == DecoratorPosition.AfterGUI ? GetHeight(messageBoxAttribute) : 0;
        }

        /// <inheritdoc/>
        public float GetHeightBeforeGUI(ref InspectorDecoratorDrawerContext context)
        {
            MessageBoxAttribute messageBoxAttribute = (MessageBoxAttribute)context.Attribute;

            return messageBoxAttribute.Position == DecoratorPosition.BeforeGUI ? GetHeight(messageBoxAttribute) : 0;
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
        private static float GetHeight(MessageBoxAttribute messageBoxAttribute)
        {
            return CoimbraEditorGUIUtility.GetMessageBoxHeight(messageBoxAttribute.Message, messageBoxAttribute.Type, messageBoxAttribute.Area, EditorGUIUtility.singleLineHeight);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void OnGUI(ref InspectorDecoratorDrawerContext context)
        {
            if (context.Position.height == 0)
            {
                return;
            }

            Rect position = context.Position;
            MessageBoxAttribute messageBoxAttribute = (MessageBoxAttribute)context.Attribute;
            CoimbraEditorGUIUtility.DrawMessageBox(position, messageBoxAttribute.Message, messageBoxAttribute.Type, messageBoxAttribute.Area);
        }
    }
}
