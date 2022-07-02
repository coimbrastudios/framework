using Coimbra.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Services.Events.Editor
{
    /// <summary>
    /// Drawer for <see cref="EventHandle"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(EventHandle))]
    public sealed class EventHandleDrawer : PropertyDrawer
    {
        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            EventHandle eventHandle = property.GetValue<EventHandle>();

            if (!eventHandle.IsValid || property.GetPropertyPathInfo().HasMultipleDifferentValues(property.serializedObject.targetObjects))
            {
                return EditorGUIUtility.singleLineHeight;
            }

            using (ListPool.Pop(out List<DelegateListener> list))
            {
                eventHandle.Service.GetListeners(in eventHandle, list);

                return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + CoimbraGUIUtility.GetDelegateListenersHeight(list, property.serializedObject.isEditingMultipleObjects);
            }
        }

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;

            using EditorGUI.PropertyScope propertyScope = new(position, label, property);
            PropertyPathInfo info = property.GetPropertyPathInfo();

            if (info.HasMultipleDifferentValues(property.serializedObject.targetObjects))
            {
                using (GUIContentPool.Pop(out GUIContent temp))
                {
                    temp.text = "-";
                    temp.tooltip = "Editing multiple different values.";
                    EditorGUI.LabelField(position, propertyScope.content, temp);
                }

                return;
            }

            EventHandle eventHandle = info.GetValue<EventHandle>(property.serializedObject.targetObject);

            if (!eventHandle.IsValid || eventHandle.Service.GetListenerCount(eventHandle.Type) == 0)
            {
                using (GUIContentPool.Pop(out GUIContent temp))
                {
                    temp.text = "Invalid";
                    EditorGUI.LabelField(position, propertyScope.content, temp);
                }

                return;
            }

            if (label == GUIContent.none)
            {
                property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, eventHandle.ToString(), true);
            }
            else
            {
                using (new ResetIndentLevelScope())
                {
                    EditorGUI.LabelField(CoimbraGUIUtility.AdjustPosition(position, InspectorArea.Field), eventHandle.ToString());
                }

                property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, propertyScope.content, true);
            }

            if (!property.isExpanded)
            {
                return;
            }

            using (new EditorGUI.IndentLevelScope())
            using (ListPool.Pop(out List<DelegateListener> list))
            using (GUIContentPool.Pop(out GUIContent temp))
            {
                temp.text = TypeString.Get(eventHandle.Type);
                eventHandle.Service.GetListeners(in eventHandle, list);

                position.y = position.yMax + EditorGUIUtility.standardVerticalSpacing;
                position.height = CoimbraGUIUtility.GetDelegateListenersHeight(list, property.serializedObject.isEditingMultipleObjects);
                CoimbraGUIUtility.DrawDelegateListeners(position, temp, list, property.serializedObject.isEditingMultipleObjects);
            }

            foreach (UnityEditor.Editor editor in ActiveEditorTracker.sharedTracker.activeEditors)
            {
                editor.Repaint();
            }
        }
    }
}
