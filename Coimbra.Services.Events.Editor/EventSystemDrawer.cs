using Coimbra.Editor;
using System;
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
        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            if (property.hasMultipleDifferentValues)
            {
                return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
            }

            float height = EditorGUIUtility.singleLineHeight;

            using (GUIContentPool.Pop(out GUIContent temp))
            using (ListPool.Pop(out List<DelegateListener> list))
            {
                foreach (KeyValuePair<Type, Event> pair in property.GetValue<EventSystem>()!.Events)
                {
                    height += EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight * 1.5f;

                    list.Clear();
                    pair.Value.GetListeners(list);
                    height += EditorGUIUtility.standardVerticalSpacing + CoimbraEditorGUIUtility.GetDelegateListenersHeight(list, false);

                    list.Clear();
                    pair.Value.GetRelevancyListeners(list);
                    height += EditorGUIUtility.standardVerticalSpacing + CoimbraEditorGUIUtility.GetDelegateListenersHeight(list, false);
                }
            }

            return height;
        }

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;

            using EditorGUI.PropertyScope propertyScope = new EditorGUI.PropertyScope(position, label, property);

            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, propertyScope.content, true);

            if (!property.isExpanded)
            {
                return;
            }

            using (new EditorGUI.IndentLevelScope())
            {
                if (property.hasMultipleDifferentValues)
                {
                    position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                    EditorGUI.LabelField(position, "Multi-edit is not supported.");

                    return;
                }

                using (GUIContentPool.Pop(out GUIContent temp))
                using (ListPool.Pop(out List<DelegateListener> list))
                {
                    foreach (KeyValuePair<Type, Event> pair in property.GetValue<EventSystem>()!.Events)
                    {
                        position.y += position.height + EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight * 0.5f;
                        position.height = EditorGUIUtility.singleLineHeight;
                        EditorGUI.LabelField(position, TypeString.Get(pair.Key), EditorStyles.boldLabel);

                        list.Clear();
                        pair.Value.GetListeners(list);

                        temp.text = "Listeners";
                        position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                        position.height = CoimbraEditorGUIUtility.GetDelegateListenersHeight(list, false);
                        CoimbraEditorGUIUtility.DrawDelegateListeners(position, temp, list, false);

                        list.Clear();
                        pair.Value.GetRelevancyListeners(list);

                        temp.text = "Relevancy Listeners";
                        position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                        position.height = CoimbraEditorGUIUtility.GetDelegateListenersHeight(list, false);
                        CoimbraEditorGUIUtility.DrawDelegateListeners(position, temp, list, false);
                    }
                }
            }
        }
    }
}
