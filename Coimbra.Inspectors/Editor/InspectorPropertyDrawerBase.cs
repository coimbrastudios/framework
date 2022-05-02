#if UNITY_EDITOR
#nullable enable

using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Inspectors.Editor
{
    /// <summary>
    /// Custom implementation of <see cref="PropertyDrawer"/> with extended functionalities.
    /// </summary>
    public abstract class InspectorPropertyDrawerBase : PropertyDrawer
    {
        internal static Action<Rect, SerializedProperty, bool> DrawCustomInspectorsHandler = null!;

        internal static Func<SerializedProperty, bool, float> GetCustomInspectorsHeightHandler = null!;

        /// <summary>
        /// If false, the initial property label and foldout will not be drawn.
        /// </summary>
        public virtual bool DrawPropertyHeader => true;

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float totalHeight = 0;

            if (DrawPropertyHeader)
            {
                totalHeight += GetCustomInspectorsHeight(property.Copy(), false);

                if (!property.isExpanded)
                {
                    return totalHeight;
                }
            }

            SerializedProperty iterator = property.Copy();
            iterator.NextVisible(true);

            float height = GetCustomInspectorsHeight(iterator, true);

            if (height > 0)
            {
                totalHeight += height + EditorGUIUtility.standardVerticalSpacing;
            }

            return totalHeight;
        }

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (DrawPropertyHeader)
            {
                DrawCustomInspectors(position, property.Copy(), false);

                if (!property.isExpanded)
                {
                    return;
                }
            }

            using (new EditorGUI.IndentLevelScope(1))
            {
                SerializedProperty iterator = property.Copy();
                iterator.NextVisible(true);
                position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                DrawCustomInspectors(position, iterator, true);
            }
        }

        /// <summary>
        /// Custom inspector implementation with extended functionalities.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static void DrawCustomInspectors(Rect position, SerializedProperty iterator, bool includeChildren)
        {
            DrawCustomInspectorsHandler(position, iterator, includeChildren);
        }

        /// <summary>
        /// Get height for <see cref="DrawCustomInspectors"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static float GetCustomInspectorsHeight(SerializedProperty iterator, bool includeChildren)
        {
            return GetCustomInspectorsHeightHandler(iterator, includeChildren);
        }
    }
}
#endif
