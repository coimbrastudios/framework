using Coimbra.Editor;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Services.Timers.Editor
{
    [CustomEditor(typeof(TimerComponent))]
    internal sealed class TimerComponentEditor : UnityEditor.Editor
    {
        private SerializedProperty _completedLoops;

        private SerializedProperty _delay;

        private SerializedProperty _rate;

        private SerializedProperty _targetLoops;

        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.FloatField("Delay", _delay.floatValue);

                float rate = _rate.floatValue;

                if (rate < 0)
                {
                    EditorGUILayout.LabelField("Rate", "Once");
                }
                else
                {
                    EditorGUILayout.FloatField("Rate", rate);
                }

                Rect position = EditorGUILayout.GetControlRect();

                using (GUIContentPool.Pop(out GUIContent temp))
                {
                    temp.text = "Loops";
                    position = EditorGUI.PrefixLabel(position, temp);
                }

                using (GUIContentPool.Pop(out GUIContent temp))
                {
                    temp.text = "/";

                    float separatorWidth = EditorStyles.label.CalcSize(temp).x;
                    position.width = position.width * 0.5f - 2 - separatorWidth - EditorGUIUtility.standardVerticalSpacing * 2;
                    EditorGUI.IntField(position, _completedLoops.intValue);
                    position.x += position.width + separatorWidth + EditorGUIUtility.standardVerticalSpacing * 2;

                    int targetLoops = _targetLoops.intValue;

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

                using (GUIContentPool.Pop(out GUIContent temp))
                {
                    temp.text = "Callback";

                    TimerComponent timerComponent = (TimerComponent)target;
                    position.height = CoimbraEditorGUIUtility.GetDelegateListenersHeight(in timerComponent.Callback, serializedObject.isEditingMultipleObjects);
                    position = EditorGUILayout.GetControlRect(false, position.height);
                    CoimbraEditorGUIUtility.DrawDelegateListeners(position, temp, in timerComponent.Callback, serializedObject.isEditingMultipleObjects);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            _completedLoops = serializedObject.FindProperty(nameof(TimerComponent.CompletedLoops));
            _delay = serializedObject.FindProperty(nameof(TimerComponent.Delay));
            _rate = serializedObject.FindProperty(nameof(TimerComponent.Rate));
            _targetLoops = serializedObject.FindProperty(nameof(TimerComponent.TargetLoops));
        }
    }
}
