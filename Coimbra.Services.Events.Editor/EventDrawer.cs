using Coimbra.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Coimbra.Services.Events.Editor
{
    [CustomPropertyDrawer(typeof(Event))]
    internal sealed class EventDrawer : PropertyDrawer
    {
        private const string ListenersProperty = "_listeners";

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;

            if (!property.isExpanded)
            {
                return height;
            }

            ReorderableList listenersList = property.FindPropertyRelative(ListenersProperty).ToReorderableList(HandleInitializeReorderableList);
            height += EditorGUIUtility.standardVerticalSpacing + listenersList.GetHeight();

            using (ListPool.Pop(out List<DelegateListener> list))
            {
                property.GetValue<Event>()!.GetRelevancyListeners(list);

                height += EditorGUIUtility.standardVerticalSpacing + CoimbraGUIUtility.GetDelegateListenersHeight(list, property.hasMultipleDifferentValues);
            }

            return height;
        }

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ReorderableList listenersList = property.FindPropertyRelative(ListenersProperty).ToReorderableList(HandleInitializeReorderableList);
            CoimbraGUIUtility.DrawListHeader(position, label, property, listenersList);

            if (!property.isExpanded)
            {
                return;
            }

            using (new EditorGUI.IndentLevelScope())
            {
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                position.height = listenersList.GetHeight();
                listenersList.DoList(EditorGUI.IndentedRect(position));

                using (GUIContentPool.Pop(out GUIContent temp))
                using (ListPool.Pop(out List<DelegateListener> list))
                {
                    bool isMultiEditing = property.hasMultipleDifferentValues;
                    temp.text = "Relevancy Listeners";
                    property.GetValue<Event>()!.GetRelevancyListeners(list);

                    position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                    position.height = CoimbraGUIUtility.GetDelegateListenersHeight(list, isMultiEditing);
                    CoimbraGUIUtility.DrawDelegateListeners(position, temp, list, isMultiEditing);
                }
            }
        }

        private static void HandleInitializeReorderableList(ReorderableList list)
        {
            list.draggable = false;
            list.displayAdd = false;
            list.displayRemove = false;
            list.footerHeight = 0;
            list.headerHeight = 0;

            list.drawElementCallback = delegate(Rect rect, int index, bool active, bool focused)
            {
                EditorGUI.PropertyField(rect, list.serializedProperty.GetArrayElementAtIndex(index), GUIContent.none);
            };

            list.elementHeightCallback = delegate(int index)
            {
                return list.serializedProperty.arraySize == 0 ? 0 : EditorGUI.GetPropertyHeight(list.serializedProperty.GetArrayElementAtIndex(index));
            };
        }
    }
}
