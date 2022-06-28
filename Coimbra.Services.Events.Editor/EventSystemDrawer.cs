using Coimbra.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Services.Events.Editor
{
    /// <summary>
    /// Drawer for <see cref="EventSystem"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(EventSystem))]
    public sealed class EventSystemDrawer : PropertyDrawer
    {
        private const string ListProperty = "_list";

        private string _filter;

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            float height = EditorGUIUtility.singleLineHeight;

            if (!property.hasMultipleDifferentValues)
            {
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            SerializedProperty listProperty = property.FindPropertyRelative(ListProperty);
            int listSize = listProperty.arraySize;
            bool hasFilter = !property.hasMultipleDifferentValues && !string.IsNullOrWhiteSpace(_filter);

            for (int i = 0; i < listSize; i++)
            {
                SerializedProperty elementProperty = listProperty.GetArrayElementAtIndex(i);

                if (!hasFilter || TryMatchSearch(elementProperty.GetValue<Event>()))
                {
                    height += EditorGUI.GetPropertyHeight(elementProperty) + EditorGUIUtility.standardVerticalSpacing + (EditorGUIUtility.singleLineHeight * 0.4f);
                }
            }

            return height;
        }

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;

            if (!EditorGUI.PropertyField(position, property, label, false))
            {
                return;
            }

            using (new EditorGUI.IndentLevelScope())
            {
                if (!property.hasMultipleDifferentValues)
                {
                    position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                    _filter = EditorGUI.TextField(position, "Filter", _filter);
                }

                bool hasFilter = !property.hasMultipleDifferentValues && !string.IsNullOrWhiteSpace(_filter);
                SerializedProperty listProperty = property.FindPropertyRelative(ListProperty);
                int listSize = listProperty.arraySize;

                for (int i = 0; i < listSize; i++)
                {
                    SerializedProperty elementProperty = listProperty.GetArrayElementAtIndex(i);

                    if (hasFilter && !TryMatchSearch(elementProperty.GetValue<Event>()))
                    {
                        continue;
                    }

                    position.y += position.height + EditorGUIUtility.standardVerticalSpacing + (EditorGUIUtility.singleLineHeight * 0.2f);
                    position.height = EditorGUI.GetPropertyHeight(elementProperty);
                    EditorGUI.PropertyField(position, elementProperty);

                    position.y += EditorGUIUtility.singleLineHeight * 0.2f;
                }
            }
        }

        private bool TryMatchSearch(Event e)
        {
            if (CoimbraGUIUtility.TryMatchSearch(_filter, e.Label))
            {
                return true;
            }

            using (ListPool.Pop(out List<DelegateListener> list))
            {
                for (int i = 0; i < e.ListenerCount; i++)
                {
                    if (CoimbraGUIUtility.TryMatchSearch(_filter, e[i].ToString()))
                    {
                        return true;
                    }

                    e.GetListenersHandler(e[i], list);
                }

                e.GetRelevancyListeners(list);

                foreach (DelegateListener listener in list)
                {
                    if (CoimbraGUIUtility.TryMatchSearch(_filter, listener.ToString()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
