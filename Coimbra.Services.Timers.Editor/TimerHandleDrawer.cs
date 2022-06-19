using Coimbra.Editor;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Services.Timers.Editor
{
    /// <summary>
    /// Drawer for <see cref="TimerHandle"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(TimerHandle))]
    public sealed class TimerHandleDrawer : PropertyDrawer
    {
        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded || !ServiceLocator.IsSet(out ITimerService timerService) || !timerService!.IsTimerActive(property.GetValue<TimerHandle>()) || property.GetPropertyPathInfo().HasMultipleDifferentValues(property.serializedObject.targetObjects))
            {
                return EditorGUIUtility.singleLineHeight;
            }

            return EditorGUIUtility.singleLineHeight * 4 + EditorGUIUtility.standardVerticalSpacing * 3;
        }

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;

            using EditorGUI.PropertyScope propertyScope = new EditorGUI.PropertyScope(position, label, property);
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

            TimerHandle timerHandle = info.GetValue<TimerHandle>(property.serializedObject.targetObject);

            if (!ServiceLocator.IsSet(out ITimerService timerService) || !timerService!.IsTimerActive(in timerHandle, out float delay, out float rate, out int targetLoops, out int completedLoops))
            {
                using (GUIContentPool.Pop(out GUIContent temp))
                {
                    temp.text = "Inactive";
                    EditorGUI.LabelField(position, propertyScope.content, temp);
                }

                return;
            }

            Rect fieldPosition = position;
            fieldPosition.xMin += EditorGUIUtility.labelWidth;
            EditorGUI.LabelField(fieldPosition, "Active");

            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, propertyScope.content, true);

            if (!property.isExpanded)
            {
                return;
            }

            using (new EditorGUI.IndentLevelScope())
            using (new EditorGUI.DisabledScope(true))
            {
                position.y += position.height + EditorGUIUtility.standardVerticalSpacing;

                EditorGUI.FloatField(position, "Delay", delay);

                position.y += position.height + EditorGUIUtility.standardVerticalSpacing;

                if (rate < 0)
                {
                    EditorGUI.LabelField(position, "Rate", "Once");
                }
                else
                {
                    EditorGUI.FloatField(position, "Rate", rate);
                }

                position.y += position.height + EditorGUIUtility.standardVerticalSpacing;

                using (GUIContentPool.Pop(out GUIContent temp))
                {
                    temp.text = "Loops";
                    position = EditorGUI.PrefixLabel(position, temp);
                }
            }

            using (GUIContentPool.Pop(out GUIContent temp))
            {
                temp.text = "/";

                float separatorWidth = EditorStyles.label.CalcSize(temp).x;
                position.width = position.width * 0.5f - 2 - separatorWidth - EditorGUIUtility.standardVerticalSpacing * 2;
                EditorGUI.IntField(position, completedLoops);
                position.x += position.width + separatorWidth + EditorGUIUtility.standardVerticalSpacing * 2;

                if (targetLoops == 0)
                {
                    EditorGUI.LabelField(position, "Infinity");
                }
                else
                {
                    EditorGUI.IntField(position, targetLoops);
                }

                position.x -= separatorWidth + EditorGUIUtility.standardVerticalSpacing;
                position.width = separatorWidth;
                EditorGUI.LabelField(position, temp);
            }
        }
    }
}
